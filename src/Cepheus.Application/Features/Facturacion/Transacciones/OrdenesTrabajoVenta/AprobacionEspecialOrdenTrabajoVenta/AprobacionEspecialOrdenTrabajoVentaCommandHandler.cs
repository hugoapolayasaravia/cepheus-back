using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobacionEspecialOrdenTrabajoVenta;

public sealed class AprobacionEspecialOrdenTrabajoVentaCommandHandler
    : IRequestHandler<AprobacionEspecialOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public AprobacionEspecialOrdenTrabajoVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<OrdenTrabajoVentaResponse> Handle(AprobacionEspecialOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var code = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var ot = await OrdenTrabajoVentaReader.LoadForUpdateAsync(_uow, code, ct);
        OrdenTrabajoVentaRules.EnsureActive(ot, "aprobación especial");

        if (ot.Stage != EtapaOrdenTrabajoVenta.Pendiente)
        {
            throw new InvalidOperationException($"La Orden de Trabajo está {ot.Stage}; la aprobación especial es para una OT Pendiente.");
        }

        if (ot.SpecialApprovalGranted)
        {
            throw new InvalidOperationException("La OT ya tiene aprobación especial.");
        }

        ot.SpecialApprovalGranted = request.Data.Granted;
        ot.SpecialApprovalBy = _currentUser.FullName;
        ot.SpecialApprovalAt = DateTime.Now;
        ot.SpecialApprovalObservations = OrdenTrabajoVentaRules.NormalizeText(request.Data.Observations);

        // Concedida: la OT se reabre para completar/ajustar las líneas sin validar la línea de crédito.
        if (request.Data.Granted)
        {
            ot.IsModifiable = true;
        }

        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, code, ct);
    }
}
