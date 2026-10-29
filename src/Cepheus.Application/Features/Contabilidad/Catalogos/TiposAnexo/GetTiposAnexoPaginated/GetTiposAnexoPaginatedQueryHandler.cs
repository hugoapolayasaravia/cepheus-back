using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.CreateTipoAnexo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.GetTiposAnexoPaginated
{
    public class GetTiposAnexoPaginatedQueryHandler
        : IRequestHandler<GetTiposAnexoPaginatedQuery, PagedResult<TipoAnexoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTiposAnexoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TipoAnexoResponse>> Handle(
            GetTiposAnexoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Catalogos.TiposAnexo.Query().AsNoTracking();

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

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<TipoAnexoResponse>
            {
                Items = paged.Items.Select(CreateTipoAnexoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
