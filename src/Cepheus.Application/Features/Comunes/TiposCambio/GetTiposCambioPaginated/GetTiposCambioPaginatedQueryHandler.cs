using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.GetTiposCambioPaginated
{
    public class GetTiposCambioPaginatedQueryHandler
        : IRequestHandler<GetTiposCambioPaginatedQuery, PagedResult<TipoCambioResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTiposCambioPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TipoCambioResponse>> Handle(
            GetTiposCambioPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.TiposCambio.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.MonedaCode))
            {
                var moneda = TipoCambioRules.Normalizar(request.MonedaCode);
                query = query.Where(t => t.MonedaCode == moneda);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(t => t.Date >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(t => t.Date <= request.ToDate.Value);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(t => t.Date).ThenBy(t => t.MonedaCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new TipoCambioResponse
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
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
