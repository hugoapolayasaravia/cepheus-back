using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.CreateProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.GetProductosPaginated
{
    public class GetProductosPaginatedQueryHandler
        : IRequestHandler<GetProductosPaginatedQuery, PagedResult<ProductoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProductosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ProductoResponse>> Handle(
            GetProductosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Productos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p =>
                    (p.TipoProductoCode + p.Code).ToLower().Contains(search) ||
                    p.Name.ToLower().Contains(search) ||
                    (p.ShortName != null && p.ShortName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoProductoCode))
            {
                var tipoProductoCode = request.TipoProductoCode.Trim().ToUpper();
                query = query.Where(p => p.TipoProductoCode == tipoProductoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.CategoryCode))
            {
                var categoryCode = request.CategoryCode.Trim().ToUpper();
                query = query.Where(p => p.CategoryCode == categoryCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.TipoProductoCode).ThenBy(p => p.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ProductoResponse>
            {
                Items = paged.Items.Select(CreateProductoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
