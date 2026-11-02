using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.CreateNoAfectoRetencion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.GetNoAfectosRetencionPaginated
{
    public class GetNoAfectosRetencionPaginatedQueryHandler
        : IRequestHandler<GetNoAfectosRetencionPaginatedQuery, PagedResult<NoAfectoRetencionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetNoAfectosRetencionPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<NoAfectoRetencionResponse>> Handle(
            GetNoAfectosRetencionPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Maestros.NoAfectosRetencion.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Ruc.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search) ||
                    x.Resolution.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                var type = request.Type.Trim().ToUpper();
                query = query.Where(x => x.Type == type);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Ruc).ThenBy(x => x.Type)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<NoAfectoRetencionResponse>
            {
                Items = paged.Items.Select(CreateNoAfectoRetencionCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
