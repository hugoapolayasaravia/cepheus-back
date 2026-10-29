using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.GetSolicitudNotaCreditoByCode
{
    public class GetSolicitudNotaCreditoByCodeQueryHandler
        : IRequestHandler<GetSolicitudNotaCreditoByCodeQuery, SolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSolicitudNotaCreditoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SolicitudNotaCreditoResponse> Handle(
            GetSolicitudNotaCreditoByCodeQuery request, CancellationToken cancellationToken)
        {
            var code = SolicitudNotaCreditoRules.Normalize(request.Code);

            var solicitud = await SolicitudNotaCreditoMapper.LoadAsync(
                _uow, request.Year.Trim(), request.Month.Trim(), code, cancellationToken);

            if (solicitud is null)
            {
                throw new KeyNotFoundException(
                    $"Solicitud de nota de crédito {SolicitudNotaCreditoRules.FormatKey(request.Year, request.Month, code)} no encontrada.");
            }

            return solicitud;
        }
    }
}
