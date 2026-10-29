using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularAprobacionOrdenTrabajoVenta;

public sealed class AnularAprobacionOrdenTrabajoVentaCommandHandler
    : IRequestHandler<AnularAprobacionOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public AnularAprobacionOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(AnularAprobacionOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query()
            .Include(x => x.Detalles)
            .Include(x => x.Aprobacion)
            .FirstOrDefaultAsync(x => x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Trabajo {code} no existe.");

        var approval = ot.Aprobacion
            ?? throw new InvalidOperationException("La Orden de Trabajo no tiene aprobación que anular.");

        if (ot.Stage == EtapaOrdenTrabajoVenta.GeneraProgramacion)
        {
            throw new InvalidOperationException(
                "La Orden de Trabajo tiene programación(es) existente(s), imposible anular su aprobación.");
        }

        if (ot.Stage != EtapaOrdenTrabajoVenta.AprobadaCredito)
        {
            throw new InvalidOperationException($"La Orden de Trabajo está {ot.Stage}; no tiene una aprobación vigente.");
        }

        OrdenTrabajoVentaRules.EnsureActive(ot, "anulación de la aprobación");

        // Contado con comprobante ya emitido: no se anula (excepto la OT por Descuento, que no emite documento propio).
        if (ot.Type != TipoOrdenTrabajoVenta.Descuento
            && approval.PaymentType == TipoPagoAprobacion.Contado
            && await OrdenTrabajoVentaDocumentos.IsIssuedAsync(_uow, approval, ct))
        {
            throw new InvalidOperationException(
                "El documento de pago ya fue generado, imposible anular la aprobación.");
        }

        // Devuelve los números a la planta (el segundo documento primero: es el último si ambos salen de la misma planta).
        var plant2 = approval.Plant2Code is null
            ? null
            : await _uow.Comunes.Plantas.Query().FirstOrDefaultAsync(p => p.Code == approval.Plant2Code, ct);

        OrdenTrabajoVentaDocumentos.Release(plant2, approval.DocumentType, approval.Document2Number);

        var plant1 = approval.Plant1Code is null
            ? null
            : await _uow.Comunes.Plantas.Query().FirstOrDefaultAsync(p => p.Code == approval.Plant1Code, ct);

        OrdenTrabajoVentaDocumentos.Release(plant1, approval.DocumentType, approval.Document1Number);

        // Bitácora.
        await _uow.Facturacion.Transacciones.OrdenesTrabajoVentaAprobacionesAnuladas.AddAsync(
            new OrdenTrabajoVentaAprobacionAnulada
            {
                OrdenTrabajoCode = code,
                Reason = request.Data.Reason.Trim(),
                Observations = OrdenTrabajoVentaRules.NormalizeText(request.Data.Observations),
                DocumentType = approval.DocumentType,
                Document1Number = approval.Document1Number,
                Document2Number = approval.Document2Number,
                Plant1Code = approval.Plant1Code,
                Plant2Code = approval.Plant2Code,
                ApprovedQuantity = approval.ApprovedQuantity,
                ApprovedBy = ot.ApprovedBy,
                ApprovedAt = ot.ApprovedAt
            }, ct);

        _uow.Facturacion.Transacciones.OrdenesTrabajoVentaAprobaciones.Remove(approval);
        ot.Aprobacion = null;

        foreach (var line in ot.Detalles)
        {
            line.ApprovedQuantity = 0m;
        }

        ot.Stage = EtapaOrdenTrabajoVenta.Pendiente;
        ot.IsModifiable = true;
        ot.ApprovedBy = null;
        ot.ApprovedAt = null;
        ot.ApprovalObservations = null;
        ot.ApprovalDelayReason = null;
        ot.ProductionPassReason = null;
        ot.SentToApprovalAt = null;
        ot.PassedToProduction = false;
        ot.ProductionPassDate = null;

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
