using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.InactivarOrdenTrabajoVenta;

public sealed class InactivarOrdenTrabajoVentaCommandHandler
    : IRequestHandler<InactivarOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public InactivarOrdenTrabajoVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<OrdenTrabajoVentaResponse> Handle(InactivarOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);

        if (ot.Stage == EtapaOrdenTrabajoVenta.Anulada)
        {
            throw new InvalidOperationException("La Orden de Trabajo está anulada, no se puede inactivar.");
        }

        if (ot.Status != EstadoOrdenTrabajoVenta.Activo)
        {
            throw new InvalidOperationException(
                $"Solo se inactiva una Orden de Trabajo Activa (hoy está {ot.Status}).");
        }

        ot.Status = EstadoOrdenTrabajoVenta.Inactivo;
        ot.InactivatedBy = _currentUser.FullName;
        ot.InactivatedAt = DateTime.Now;
        ot.InactivationReason = request.Data.Reason.Trim();

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
