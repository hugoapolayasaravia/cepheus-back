using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.CreateCategoriaProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.GetCategoriasProductoPaginated
{
    public class GetCategoriasProductoPaginatedQueryHandler
        : IRequestHandler<GetCategoriasProductoPaginatedQuery, PagedResult<CategoriaProductoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCategoriasProductoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CategoriaProductoResponse>> Handle(
            GetCategoriasProductoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.CategoriasProducto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.Name.ToLower().Contains(search) ||
                    (x.FirthCode != null && x.FirthCode.ToLower().Contains(search)));
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

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo).
            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<CategoriaProductoResponse>
            {
                Items = paged.Items.Select(CreateCategoriaProductoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
