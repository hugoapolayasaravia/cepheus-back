using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>Aprobación de la OT: armado de la aprobación automática y aplicación sobre la OT.</summary>
public static class OrdenTrabajoVentaAprobaciones
{
    /// <summary>Planta, motivo de guía y formas de pago que el SP ges_sp_AgregaOrdenTrabajoAprueba fija en la aprobación automática.</summary>
    public const string AutoPlantCode = "OF";
    public const string AutoGuideMotivoCode = "01";
    public const string AnticipoFormaPagoCode = "32";
    public const string SaldoFormaPagoCode = "17";

    public const string DescuentoCreditsObservations = "OT. POR DESCUENTO POR ANTICIPO";
    public const string SaldoCreditsObservations = "OT. POR SALDO";
    public const string AutoCreditsObservations = "Aprobación automática";

    /// <summary>Productos de la OT que habilitan el doble documento: bombeo (04) en Concreto y transporte (07) en los demás.</summary>
    public const string BombeoProductoTipo = "04";
    public const string TransporteProductoTipo = "07";

    /// <summary>Factura si el cliente tiene RUC de 11 dígitos, Boleta en otro caso (PB: len(ruc) = 11).</summary>
    public static TipoDocumentoAprobacion DocumentTypeFor(Cliente cliente)
        => cliente.DocumentNumber?.Trim().Length == 11
            ? TipoDocumentoAprobacion.Factura
            : TipoDocumentoAprobacion.Boleta;

    /// <summary>
    /// Cliente cuyas OT siempre pasan por Créditos (maestro de clientes: "Requiere Aprobación O.T." = Aprobacon_Ot 'S')
    /// y no es VIP. Los demás se aprueban solos al bloquear la OT (SP: tipo N y (Aprobacon_Ot = 'N' o cliente VIP)).
    /// </summary>
    public static bool RequiresManualApproval(Cliente cliente)
        => !cliente.IsVip && cliente.RequiresWorkOrderApproval == true;

    /// <summary>Negocio de la OT: el de la cotización o, si no hay, el de la primera línea (PB: select top 1 codigo_neg).</summary>
    public static string? NegocioOf(OrdenTrabajoVenta ot)
        => ot.CotizacionNegocioCode
           ?? ot.Detalles.Where(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado)
               .OrderBy(d => d.Item)
               .Select(d => d.NegocioCode)
               .FirstOrDefault();

    /// <summary>
    /// El doble documento (factura de concreto + factura del bombeo/transporte) solo aplica a Factura y si la OT tiene
    /// líneas de bombeo (04, Concreto) o de transporte (07, otros negocios) — PB: ib_doble / dw_4 filtrado por tpr.
    /// </summary>
    public static bool AllowsTwoDocuments(OrdenTrabajoVenta ot, TipoDocumentoAprobacion documentType)
    {
        if (documentType != TipoDocumentoAprobacion.Factura)
        {
            return false;
        }

        var tipo = NegocioOf(ot) == OrdenTrabajoVentaRules.NegocioConcreto ? BombeoProductoTipo : TransporteProductoTipo;

        return ot.Detalles.Any(d => d.Status != EstadoDetalleOrdenTrabajoVenta.Anulado && d.ProductoTipoCode == tipo);
    }

    /// <summary>Condición de pago de la obra (la de la obra y, si no tiene, la del cliente).</summary>
    public static async Task<(Cliente Cliente, Obra Obra, bool IsCredit)> GetPaymentContextAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta ot,
        CancellationToken ct)
    {
        var cliente = await uow.Facturacion.Maestros.Clientes
            .Query().AsNoTracking()
            .Include(c => c.FormaPagoVenta)
            .FirstOrDefaultAsync(c => c.Code == ot.ClienteCode, ct)
            ?? throw new KeyNotFoundException($"El cliente {ot.ClienteCode} no existe.");

        var obra = await uow.Facturacion.Maestros.Obras
            .Query().AsNoTracking()
            .Include(o => o.FormaPagoVenta)
            .Include(o => o.TipoValorizacion)
            .FirstOrDefaultAsync(o => o.ClienteCode == ot.ClienteCode && o.Code == ot.ObraCode, ct)
            ?? throw new KeyNotFoundException($"La obra {ot.ClienteCode}/{ot.ObraCode} no existe.");

        var isCredit = (obra.FormaPagoVenta ?? cliente.FormaPagoVenta).IsCredit;

        return (cliente, obra, isCredit);
    }

    /// <summary>true si el SP aprueba solo este tipo de OT: Normal, Descuento (copia el anticipo) y Saldo (copia la OT origen).</summary>
    public static bool IsAutoApprovableType(TipoOrdenTrabajoVenta type)
        => type is TipoOrdenTrabajoVenta.Normal or TipoOrdenTrabajoVenta.Descuento or TipoOrdenTrabajoVenta.Saldo;

    /// <summary>
    /// Aprobación automática (PB: ges_sp_AgregaOrdenTrabajoAprueba). No asigna número de documento (el SP deja los
    /// documentos en blanco).
    ///  - Normal: Factura (RUC) o Boleta; obra SIN valorización => Crédito y flujo N; con valorización => Contado y flujo P.
    ///  - Descuento: copia la aprobación del anticipo ("OT. POR DESCUENTO POR ANTICIPO", forma de pago 32).
    ///  - Saldo: copia la aprobación de la OT origen ("OT. POR SALDO"); si era Contado queda a Crédito, valorización D,
    ///    1 día y forma de pago 17.
    /// </summary>
    public static async Task<OrdenTrabajoVentaAprobacion> BuildAutoAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta ot,
        Cliente cliente,
        Obra obra,
        CancellationToken ct)
    {
        var plantOk = await uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == AutoPlantCode, ct);
        var guideOk = await uow.Comunes.MotivosDevolucion.Query().AsNoTracking().AnyAsync(m => m.Code == AutoGuideMotivoCode, ct);

        var ap = new OrdenTrabajoVentaAprobacion
        {
            OrdenTrabajoCode = ot.Code,
            CreditStatus = EstadoCreditoAprobacion.Aprobado,
            Plant1Code = plantOk ? AutoPlantCode : null,
            Plant2Code = plantOk ? AutoPlantCode : null,
            GuideMotivoCode = guideOk ? AutoGuideMotivoCode : null,
            HasTwoDocuments = false,
            CreditsObservations = AutoCreditsObservations
        };

        if (ot.Type is TipoOrdenTrabajoVenta.Descuento or TipoOrdenTrabajoVenta.Saldo)
        {
            var origin = await uow.Facturacion.Transacciones.OrdenesTrabajoVentaAprobaciones
                .Query().AsNoTracking()
                .FirstOrDefaultAsync(a => a.OrdenTrabajoCode == ot.OriginCode, ct)
                ?? throw new InvalidOperationException(
                    $"La OT origen {ot.OriginCode} no tiene aprobación: no se puede aprobar la OT {ot.Type}.");

            ap.DocumentType = origin.DocumentType;
            ap.PaymentType = origin.PaymentType;
            ap.DocumentFlow = origin.DocumentFlow;
            ap.ValorizationCode = origin.ValorizationCode;
            ap.ValorizationWeekday = origin.ValorizationWeekday;
            ap.ValorizationDays = origin.ValorizationDays;
            ap.FormaPagoVentaCode = origin.FormaPagoVentaCode;

            if (ot.Type == TipoOrdenTrabajoVenta.Descuento)
            {
                var formaOk = await uow.Facturacion.Catalogos.FormasPagoVenta
                    .Query().AsNoTracking().AnyAsync(f => f.Code == AnticipoFormaPagoCode, ct);

                ap.CreditsObservations = DescuentoCreditsObservations;
                ap.FormaPagoVentaCode = formaOk ? AnticipoFormaPagoCode : origin.FormaPagoVentaCode;
            }
            else
            {
                var formaOk = await uow.Facturacion.Catalogos.FormasPagoVenta
                    .Query().AsNoTracking().AnyAsync(f => f.Code == SaldoFormaPagoCode, ct);

                ap.CreditsObservations = SaldoCreditsObservations;

                if (origin.PaymentType == TipoPagoAprobacion.Contado)
                {
                    ap.PaymentType = TipoPagoAprobacion.Credito;
                    ap.DocumentFlow = FlujoDocumentoAprobacion.SinDocumento;
                    ap.ValorizationCode = "D";
                    ap.ValorizationWeekday = null;
                    ap.ValorizationDays = 1;
                    ap.FormaPagoVentaCode = formaOk ? SaldoFormaPagoCode : origin.FormaPagoVentaCode;
                }
            }

            return ap;
        }

        ap.DocumentType = DocumentTypeFor(cliente);

        var valoriza = obra.RequiresValorizacion == true && !string.IsNullOrWhiteSpace(obra.TipoValorizacionCode);

        if (valoriza)
        {
            ap.ValorizationCode = obra.TipoValorizacionCode!.Trim();
            ap.ValorizationWeekday = obra.ScheduledWeekday;
            ap.ValorizationDays = obra.TipoValorizacion?.Days > 0 ? obra.TipoValorizacion!.Days : null;
            ap.PaymentType = TipoPagoAprobacion.Contado;
            ap.DocumentFlow = FlujoDocumentoAprobacion.TipoP;
        }
        else
        {
            ap.ValorizationCode = "N";
            ap.PaymentType = TipoPagoAprobacion.Credito;
            ap.DocumentFlow = FlujoDocumentoAprobacion.SinDocumento;
        }

        ap.FormaPagoVentaCode = obra.FormaPagoVentaCode ?? cliente.FormaPagoVentaCode;

        return ap;
    }

    /// <summary>
    /// Aplica la aprobación sobre la OT: etapa C, cantidades aprobadas por línea y datos del aprobador. Con el crédito
    /// Rechazado la cantidad aprobada es 0 (PB: "if is_estC = 'R' then id_cantidad = 0"); la OT igual pasa a etapa C.
    /// </summary>
    public static void ApplyToOrder(
        OrdenTrabajoVenta ot,
        OrdenTrabajoVentaAprobacion approval,
        string? user,
        string? observations,
        bool passToProduction,
        string? productionPassReason = null,
        string? delayReason = null)
    {
        var now = DateTime.Now;
        var rejected = approval.CreditStatus == EstadoCreditoAprobacion.Rechazado;

        foreach (var line in ot.Detalles.Where(l => l.Status != EstadoDetalleOrdenTrabajoVenta.Anulado))
        {
            line.ApprovedQuantity = rejected ? 0m : line.Quantity;
        }

        approval.ApprovedQuantity = rejected
            ? 0m
            : ot.Detalles.Where(l => l.Status != EstadoDetalleOrdenTrabajoVenta.Anulado).Sum(l => l.Quantity);
        approval.ApprovedAt = now;

        ot.Aprobacion = approval;
        ot.Stage = EtapaOrdenTrabajoVenta.AprobadaCredito;
        ot.IsModifiable = false;
        ot.ApprovedBy = user;
        ot.ApprovedAt = now;
        ot.ApprovalObservations = OrdenTrabajoVentaRules.NormalizeText(observations);
        ot.ApprovalDelayReason = OrdenTrabajoVentaRules.NormalizeText(delayReason);
        ot.PassedToProduction = passToProduction;
        ot.ProductionPassDate = passToProduction ? now : null;
        ot.ProductionPassReason = passToProduction ? null : OrdenTrabajoVentaRules.NormalizeText(productionPassReason);
    }

    public static void EnsureHasLines(OrdenTrabajoVenta ot)
    {
        if (!ot.Detalles.Any(l => l.Status != EstadoDetalleOrdenTrabajoVenta.Anulado))
        {
            throw new InvalidOperationException("La Orden de Trabajo no tiene detalle.");
        }
    }
}
