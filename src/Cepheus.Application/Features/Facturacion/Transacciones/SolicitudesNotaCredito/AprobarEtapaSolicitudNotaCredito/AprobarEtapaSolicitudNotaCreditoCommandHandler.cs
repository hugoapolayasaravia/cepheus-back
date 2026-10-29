using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AprobarEtapaSolicitudNotaCredito
{
    public class AprobarEtapaSolicitudNotaCreditoCommandHandler
        : IRequestHandler<AprobarEtapaSolicitudNotaCreditoCommand, SolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public AprobarEtapaSolicitudNotaCreditoCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public Task<SolicitudNotaCreditoResponse> Handle(
            AprobarEtapaSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
            => SolicitudNotaCreditoStageActions.ExecuteAsync(
                _uow, _currentUser,
                request.Year.Trim(), request.Month.Trim(), request.Code,
                request.Stage, request.Observation, approve: true,
                cancellationToken);
    }
}
