using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.GetTipoPagoByCode
{
    public class GetTipoPagoByCodeQueryHandler : IRequestHandler<GetTipoPagoByCodeQuery, TipoPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoPagoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPagoResponse> Handle(GetTipoPagoByCodeQuery request, CancellationToken cancellationToken)
            => await TipoPagoProjection.LoadAsync(_uow, request.Code.Trim(), cancellationToken)
               ?? throw new KeyNotFoundException($"Tipo de pago {request.Code} no encontrado.");
    }
}
