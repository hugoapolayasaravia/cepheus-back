using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.CreateUsuarioSolicitudNotaCredito
{
    public class CreateUsuarioSolicitudNotaCreditoCommandHandler
        : IRequestHandler<CreateUsuarioSolicitudNotaCreditoCommand, UsuarioSolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUsuarioSolicitudNotaCreditoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UsuarioSolicitudNotaCreditoResponse> Handle(
            CreateUsuarioSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            var negocioCode = request.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(request.Stage);

            var usuario = new UsuarioSolicitudNotaCredito
            {
                NegocioCode = negocioCode,
                Stage = stage,
                UserId = request.UserId,
                IsPrincipal = request.IsPrincipal,
                CanEnter = request.CanEnter,
                CanManage = request.CanManage,
                CanApprove = request.CanApprove,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true
            };

            await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.AddAsync(usuario, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return (await UsuarioSolicitudNotaCreditoProjection.LoadAsync(
                _uow, negocioCode, stage, request.UserId, cancellationToken))!;
        }
    }
}
