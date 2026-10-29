using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobarOrdenTrabajoVenta;

/// <summary>
/// Aprobación manual de Créditos (PB: w_ges_otrabajo_aprobacion). Créditos NO valida la línea de crédito aquí: esa
/// validación ocurre al grabar las líneas de la OT (ver OrdenTrabajoVentaCredito.IsRequired).
/// </summary>
public sealed class AprobarOrdenTrabajoVentaCommandHandler
    : IRequestHandler<AprobarOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private const string PaymentMethodCash = "01";
    private const string PaymentMethodNoBank1 = "04";
    private const string PaymentMethodNoBank2 = "10";

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AprobarOrdenTrabajoVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<OrdenTrabajoVentaResponse> Handle(AprobarOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);
        var d = request.Data;

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);

        OrdenTrabajoVentaRules.EnsureActive(ot, "aprobación");

        if (ot.Stage != EtapaOrdenTrabajoVenta.Pendiente)
        {
            throw new InvalidOperationException($"La Orden de Trabajo está {ot.Stage}; solo se aprueba una OT Pendiente.");
        }

        OrdenTrabajoVentaAprobaciones.EnsureHasLines(ot);

        var yaAprobada = await _uow.Facturacion.Transacciones.OrdenesTrabajoVentaAprobaciones
            .Query().AsNoTracking().AnyAsync(a => a.OrdenTrabajoCode == code, ct);

        if (yaAprobada)
        {
            throw new InvalidOperationException("La Orden de Trabajo ya tiene una aprobación.");
        }

        var (cliente, obra, _) = await OrdenTrabajoVentaAprobaciones.GetPaymentContextAsync(_uow, ot, ct);

        var negocio = OrdenTrabajoVentaAprobaciones.NegocioOf(ot);
        var isConcreto = negocio == OrdenTrabajoVentaRules.NegocioConcreto;

        // Fuera de Concreto la aprobación solo se hace con la OT enviada a aprobación (bloqueada). En Concreto puede
        // aprobarse también desbloqueada (PB: Créditos aprueba OT de concreto al momento del pedido).
        if (ot.IsModifiable && !isConcreto)
        {
            throw new InvalidOperationException("La Orden de Trabajo no fue enviada a aprobación: bloquéela primero.");
        }

        var rejected = d.CreditStatus == EstadoCreditoAprobacion.Rechazado;
        var isContado = !rejected && d.PaymentType == TipoPagoAprobacion.Contado;
        var paymentType = rejected ? TipoPagoAprobacion.Credito : d.PaymentType;

        if (!rejected && ot.Total <= 0m)
        {
            throw new InvalidOperationException("No se puede aprobar una OT con cantidad/total en cero.");
        }

        // ---- demora: 1 día o más desde que se envió a aprobación exige motivo
        var sentAt = ot.SentToApprovalAt ?? ot.UpdatedAt ?? ot.CreatedAt;
        var delayReason = OrdenTrabajoVentaRules.NormalizeText(d.DelayReason);

        if (!rejected && (DateTime.Now - sentAt).TotalDays >= 1d && delayReason is null)
        {
            throw new InvalidOperationException(
                "Pasó un día o más desde que la OT se envió a aprobación: indique el motivo de la demora (DelayReason).");
        }

        // ---- documento
        var documentType = d.DocumentType ?? OrdenTrabajoVentaAprobaciones.DocumentTypeFor(cliente);

        if (documentType == TipoDocumentoAprobacion.Factura && cliente.DocumentNumber?.Trim().Length != 11)
        {
            throw new InvalidOperationException("La Factura exige un cliente con RUC (11 dígitos); use Boleta.");
        }

        if (!rejected && !isContado && cliente.RequiresCashOnly)
        {
            throw new InvalidOperationException("El cliente solo compra al contado: no se puede aprobar a Crédito.");
        }

        var hasTwoDocuments = isContado && d.HasTwoDocuments;

        if (hasTwoDocuments && !OrdenTrabajoVentaAprobaciones.AllowsTwoDocuments(ot, documentType))
        {
            throw new InvalidOperationException(
                "El doble documento solo aplica a Factura y a OT con líneas de bombeo (Concreto) o transporte (otros negocios).");
        }

        // ---- forma de pago de venta
        var formaCode = OrdenTrabajoVentaRules.NormalizeFixedOrNull(d.FormaPagoVentaCode, 2)
                        ?? obra.FormaPagoVentaCode
                        ?? cliente.FormaPagoVentaCode;

        var forma = await _uow.Facturacion.Catalogos.FormasPagoVenta
            .Query().AsNoTracking()
            .FirstOrDefaultAsync(f => f.Code == formaCode, ct)
            ?? throw new KeyNotFoundException($"La forma de pago {formaCode} no existe.");

        if (!forma.IsActive)
        {
            throw new InvalidOperationException($"La forma de pago {formaCode} está inactiva.");
        }

        if (isContado && formaCode == OrdenTrabajoVentaAprobaciones.AnticipoFormaPagoCode)
        {
            throw new InvalidOperationException(
                $"La forma de pago {formaCode} (anticipo) no se puede usar con una aprobación al Contado.");
        }

        // ---- valorización
        var valCode = OrdenTrabajoVentaRules.NormalizeOrNull(d.ValorizationCode)
                      ?? (obra.RequiresValorizacion == true ? obra.TipoValorizacionCode?.Trim().ToUpperInvariant() : null)
                      ?? "N";

        DiaSemana? weekday = null;
        int? days = null;

        if (valCode != "N")
        {
            var tipoVal = await _uow.Facturacion.Catalogos.TiposValorizacion
                .Query().AsNoTracking()
                .FirstOrDefaultAsync(v => v.Code == valCode, ct)
                ?? throw new KeyNotFoundException($"El tipo de valorización {valCode} no existe.");

            if (!tipoVal.IsActive)
            {
                throw new InvalidOperationException($"El tipo de valorización {valCode} está inactivo.");
            }

            weekday = d.ValorizationWeekday ?? obra.ScheduledWeekday;
            days = d.ValorizationDays ?? (tipoVal.Days > 0 ? tipoVal.Days : null);

            if (valCode == "S" && weekday is null)
            {
                throw new InvalidOperationException("La valorización semanal (S) exige el día de la semana.");
            }

            if (valCode == "O" && (days is null || days <= 0))
            {
                throw new InvalidOperationException("La valorización por días (O) exige el número de días mayor a cero.");
            }
        }

        // ---- guía, banco, forma de cobro
        var guideCode = OrdenTrabajoVentaRules.NormalizeOrNull(d.GuideMotivoCode);

        if (guideCode is not null
            && !await _uow.Comunes.MotivosDevolucion.Query().AsNoTracking().AnyAsync(m => m.Code == guideCode, ct))
        {
            throw new KeyNotFoundException($"El motivo de guía {guideCode} no existe.");
        }

        var methodCode = OrdenTrabajoVentaRules.NormalizeOrNull(d.PaymentMethodCode);
        var bankCode = OrdenTrabajoVentaRules.NormalizeOrNull(d.BankCode);
        var paymentNumber = OrdenTrabajoVentaRules.NormalizeText(d.PaymentDocumentNumber);

        if (isContado)
        {
            if (methodCode is null)
            {
                throw new InvalidOperationException("En Contado debe indicar la forma de cobro.");
            }

            if (methodCode is not (PaymentMethodCash or PaymentMethodNoBank1 or PaymentMethodNoBank2) && bankCode is null)
            {
                throw new InvalidOperationException($"La forma de cobro {methodCode} exige banco.");
            }

            if (methodCode != PaymentMethodCash && paymentNumber is null)
            {
                throw new InvalidOperationException($"La forma de cobro {methodCode} exige el N° de pago.");
            }

            // Forma 10: el N° de pago debe ser un comprobante ya emitido (se valida cuando exista el módulo de comprobantes).
        }
        else
        {
            methodCode = null;
            bankCode = null;
            paymentNumber = null;
        }

        if (bankCode is not null
            && !await _uow.Comunes.Bancos.Query().AsNoTracking().AnyAsync(b => b.Code == bankCode && b.IsActive, ct))
        {
            throw new KeyNotFoundException($"El banco {bankCode} no existe o está inactivo.");
        }

        var flow = rejected || !isContado
            ? FlujoDocumentoAprobacion.SinDocumento
            : d.DocumentFlow ?? FlujoDocumentoAprobacion.TipoP;

        var approval = new OrdenTrabajoVentaAprobacion
        {
            OrdenTrabajoCode = code,
            CreditStatus = d.CreditStatus,
            DocumentType = documentType,
            PaymentType = paymentType,
            DocumentFlow = flow,
            ValorizationCode = valCode,
            ValorizationWeekday = weekday,
            ValorizationDays = days,
            FormaPagoVentaCode = formaCode,
            GuideMotivoCode = guideCode,
            PaymentMethodCode = methodCode,
            BankCode = bankCode,
            PaymentDocumentNumber = paymentNumber,
            CreditsObservations = OrdenTrabajoVentaRules.NormalizeText(d.CreditsObservations),
            HasTwoDocuments = hasTwoDocuments
        };

        // ---- numeración por planta (se guarda junto con la aprobación)
        if (isContado)
        {
            var plant1 = await LoadPlantAsync(OrdenTrabajoVentaRules.NormalizeFixed(d.Plant1Code!, 2), ct);
            approval.Plant1Code = plant1.Code;
            approval.Document1Number = await OrdenTrabajoVentaDocumentos.TakeAsync(
                _uow, plant1, documentType, d.Document1Number, code, cliente.Code, ct);

            if (hasTwoDocuments)
            {
                var plant2Code = OrdenTrabajoVentaRules.NormalizeFixed(d.Plant2Code!, 2);
                var plant2 = plant2Code == plant1.Code ? plant1 : await LoadPlantAsync(plant2Code, ct);

                approval.Plant2Code = plant2.Code;
                approval.Document2Number = await OrdenTrabajoVentaDocumentos.TakeAsync(
                    _uow, plant2, documentType, d.Document2Number, code, cliente.Code, ct);

                if (approval.Document1Number == approval.Document2Number)
                {
                    throw new InvalidOperationException("Los dos documentos deben tener números distintos.");
                }
            }
        }

        // ---- pase a producción: por defecto no en Concreto al Contado; con motivo cuando no pasa
        var passToProduction = !rejected && (d.PassToProduction ?? !(isConcreto && isContado));

        OrdenTrabajoVentaAprobaciones.ApplyToOrder(
            ot,
            approval,
            _currentUser.FullName,
            d.ApprovalObservations,
            passToProduction,
            d.ProductionPassReason,
            delayReason);

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }

    /// <summary>Planta tracked: su correlativo se actualiza en la misma transacción.</summary>
    private async Task<Planta> LoadPlantAsync(string plantCode, CancellationToken ct)
    {
        var planta = await _uow.Comunes.Plantas
            .Query()
            .FirstOrDefaultAsync(p => p.Code == plantCode, ct)
            ?? throw new KeyNotFoundException($"La planta {plantCode} no existe.");

        if (!planta.IsActive)
        {
            throw new InvalidOperationException($"La planta {plantCode} está inactiva.");
        }

        return planta;
    }
}
