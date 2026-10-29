using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.GetMotivoSolicitudNotaCreditoByCode
{
    public class GetMotivoSolicitudNotaCreditoByCodeQueryHandler
        : IRequestHandler<GetMotivoSolicitudNotaCreditoByCodeQuery, MotivoSolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivoSolicitudNotaCreditoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoSolicitudNotaCreditoResponse> Handle(
            GetMotivoSolicitudNotaCreditoByCodeQuery request, CancellationToken cancellationToken)
        {
            var motivo = await MotivoSolicitudNotaCreditoProjection.LoadAsync(_uow, request.Code.Trim(), cancellationToken);

            if (motivo is null)
            {
                throw new KeyNotFoundException($"Motivo de solicitud de nota de crédito {request.Code} no encontrado.");
            }

            return motivo;
        }
    }
}
