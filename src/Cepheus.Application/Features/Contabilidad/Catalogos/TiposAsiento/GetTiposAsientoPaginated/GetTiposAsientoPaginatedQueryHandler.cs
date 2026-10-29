using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.GetTiposAsientoPaginated
{
    public class GetTiposAsientoPaginatedQueryHandler
        : IRequestHandler<GetTiposAsientoPaginatedQuery, PagedResult<TipoAsientoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTiposAsientoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TipoAsientoResponse>> Handle(
            GetTiposAsientoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Catalogos.TiposAsiento.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (request.ExchangeRateType.HasValue)
            {
                query = query.Where(x => x.ExchangeRateType == request.ExchangeRateType.Value);
            }

            if (request.Valuation.HasValue)
            {
                query = query.Where(x => x.Valuation == request.Valuation.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo).
            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<TipoAsientoResponse>
            {
                Items = paged.Items.Select(CreateTipoAsientoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
