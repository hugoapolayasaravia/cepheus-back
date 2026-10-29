using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Monedas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Monedas.GetMonedasPaginated
{
    public class GetMonedasPaginatedQueryHandler
        : IRequestHandler<GetMonedasPaginatedQuery, PagedResult<MonedaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetMonedasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<MonedaResponse>> Handle(
            GetMonedasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.Monedas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(m =>
                    m.Code.ToLower().Contains(search) ||
                    m.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(m => m.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(m => m.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(m => new MonedaResponse
            {
                Code = m.Code,
                Name = m.Name,
                Symbol = m.Symbol,
                NumericCode = m.NumericCode,
                DecimalPlaces = m.DecimalPlaces,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedBy = m.UpdatedBy,
                RowVersion = m.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
