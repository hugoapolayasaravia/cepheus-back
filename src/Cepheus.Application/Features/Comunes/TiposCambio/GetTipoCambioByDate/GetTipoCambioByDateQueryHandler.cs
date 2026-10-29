using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.GetTipoCambioByDate
{
    public class GetTipoCambioByDateQueryHandler : IRequestHandler<GetTipoCambioByDateQuery, TipoCambioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoCambioByDateQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCambioResponse> Handle(GetTipoCambioByDateQuery request, CancellationToken cancellationToken)
        {
            var moneda = TipoCambioRules.Normalizar(request.MonedaCode);

            var tipoCambio = await _uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.MonedaCode == moneda && t.Date == request.Date)
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
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoCambio is null)
            {
                throw new KeyNotFoundException($"No existe tipo de cambio de {moneda} registrado para la fecha {request.Date:yyyy-MM-dd}.");
            }

            return tipoCambio;
        }
    }
}
