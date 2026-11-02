using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GetCancelacionVentaByCode
{
    public class GetCancelacionVentaByCodeQueryHandler
        : IRequestHandler<GetCancelacionVentaByCodeQuery, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCancelacionVentaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            GetCancelacionVentaByCodeQuery request, CancellationToken cancellationToken)
            => await CancelacionVentaMapper.LoadAsync(_uow, request.Code.Trim(), cancellationToken)
               ?? throw new KeyNotFoundException($"Cancelación {request.Code} no encontrada.");
    }
}
