using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.DesbloquearOrdenTrabajoVenta;

public sealed class DesbloquearOrdenTrabajoVentaCommandHandler
    : IRequestHandler<DesbloquearOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public DesbloquearOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(DesbloquearOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureActive(ot, "desbloqueo");

        if (ot.Stage != EtapaOrdenTrabajoVenta.Pendiente)
        {
            throw new InvalidOperationException(
                $"La Orden de Trabajo está {ot.Stage}; solo se desbloquea una OT pendiente de aprobación (si está aprobada, anule primero la aprobación).");
        }

        if (ot.IsModifiable)
        {
            throw new InvalidOperationException("La Orden de Trabajo ya está desbloqueada.");
        }

        ot.IsModifiable = true;

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
