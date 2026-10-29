using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.GetUsuarioSolicitudNotaCreditoByKey
{
    public class GetUsuarioSolicitudNotaCreditoByKeyQueryHandler
        : IRequestHandler<GetUsuarioSolicitudNotaCreditoByKeyQuery, UsuarioSolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUsuarioSolicitudNotaCreditoByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UsuarioSolicitudNotaCreditoResponse> Handle(
            GetUsuarioSolicitudNotaCreditoByKeyQuery request, CancellationToken cancellationToken)
        {
            var negocioCode = request.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(request.Stage);

            var usuario = await UsuarioSolicitudNotaCreditoProjection.LoadAsync(
                _uow, negocioCode, stage, request.UserId, cancellationToken);

            if (usuario is null)
            {
                throw new KeyNotFoundException(
                    $"Usuario {request.UserId} no está asignado a la etapa {stage} del negocio {negocioCode}.");
            }

            return usuario;
        }
    }
}
