using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ActivarOrdenTrabajoVenta;

public sealed class ActivarOrdenTrabajoVentaCommandHandler
    : IRequestHandler<ActivarOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public ActivarOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(ActivarOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);

        if (ot.Stage == EtapaOrdenTrabajoVenta.Anulada)
        {
            throw new InvalidOperationException("La Orden de Trabajo está anulada, no se puede activar.");
        }

        if (ot.Status != EstadoOrdenTrabajoVenta.Inactivo)
        {
            throw new InvalidOperationException(
                $"Solo se activa una Orden de Trabajo Inactiva (hoy está {ot.Status}).");
        }

        ot.Status = EstadoOrdenTrabajoVenta.Activo;
        ot.InactivatedBy = null;
        ot.InactivatedAt = null;
        ot.InactivationReason = null;

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
