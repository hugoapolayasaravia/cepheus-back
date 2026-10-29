using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularOrdenTrabajoVenta;

public sealed class AnularOrdenTrabajoVentaCommandHandler
    : IRequestHandler<AnularOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AnularOrdenTrabajoVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<OrdenTrabajoVentaResponse> Handle(AnularOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);

        switch (ot.Stage)
        {
            case EtapaOrdenTrabajoVenta.Anulada:
                throw new InvalidOperationException("La Orden de Trabajo ya está anulada.");

            case EtapaOrdenTrabajoVenta.AprobadaCredito:
                throw new InvalidOperationException(
                    "La Orden de Trabajo está aprobada: primero anule su aprobación (Créditos).");

            case EtapaOrdenTrabajoVenta.GeneraProgramacion:
                throw new InvalidOperationException(
                    "La Orden de Trabajo tiene programación(es) existente(s), imposible su anulación.");
        }

        OrdenTrabajoVentaRules.EnsureActive(ot, "anulación");

        // Devuelve a la cotización lo consumido por la OT (PB: DeliveredQuantity -= cantidad).
        var cot = await OrdenTrabajoVentaCotizacion.LoadAsync(_uow, ot, ct);

        foreach (var line in ot.Detalles.Where(l => l.Status != EstadoDetalleOrdenTrabajoVenta.Anulado))
        {
            if (cot is not null && line.NegocioCode == cot.Cotizacion.NegocioCode)
            {
                cot.Return(line.ProductoTipoCode, line.ProductoCode, line.Quantity);
            }

            line.Status = EstadoDetalleOrdenTrabajoVenta.Anulado;
        }

        // Libera las Hojas de Corte vinculadas.
        var hojas = await _uow.Facturacion.Transacciones.HojasCorte
            .Query()
            .Where(h => h.OrdenTrabajoCode == code)
            .ToListAsync(ct);

        foreach (var hc in hojas)
        {
            hc.OrdenTrabajoCode = null;
            hc.Status = request.Data.AnularHojasCorte ? EstadoHojaCorte.Anulado : EstadoHojaCorte.Pendiente;
        }

        // El Total de la cabecera se conserva (histórico); las líneas quedan anuladas.
        ot.Stage = EtapaOrdenTrabajoVenta.Anulada;
        ot.IsModifiable = false;
        ot.PurchaseOrderNumber = null;
        ot.CanceledBy = _currentUser.FullName;
        ot.CanceledAt = DateTime.Now;
        ot.CancelReason = request.Data.Reason.Trim();

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
