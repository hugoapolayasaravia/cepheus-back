using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.GetTipoCambioById
{
    public class GetTipoCambioByIdQueryHandler : IRequestHandler<GetTipoCambioByIdQuery, TipoCambioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoCambioByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCambioResponse> Handle(GetTipoCambioByIdQuery request, CancellationToken cancellationToken)
        {
            var tipoCambio = await _uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.Id == request.Id)
                .Select(t => new TipoCambioResponse
                {
                    Id = t.Id,
                    MonedaCode = t.MonedaCode,
                    MonedaName = t.Moneda.Name,
                    Date = t.Date,
                    SellRate = t.SellRate,
                    BuyRate = t.BuyRate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoCambio is null)
            {
                throw new KeyNotFoundException($"Tipo de cambio {request.Id} no encontrado.");
            }

            return tipoCambio;
        }
    }
}
