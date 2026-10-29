using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.GetStockProductosPaginated
{
    public class GetStockProductosPaginatedQueryHandler
        : IRequestHandler<GetStockProductosPaginatedQuery, PagedResult<StockProductoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetStockProductosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<StockProductoResponse>> Handle(
            GetStockProductosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.StockProductos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = request.PlantaCode.Trim().ToUpper();
                query = query.Where(x => x.PlantaCode == planta);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoProductoCode))
            {
                var tipo = request.TipoProductoCode.Trim().ToUpper();
                query = query.Where(x => x.TipoProductoCode == tipo);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoCode))
            {
                var code = request.ProductoCode.Trim().ToUpper();
                query = query.Where(x => x.ProductoCode == code);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.PlantaCode).ThenBy(x => x.TipoProductoCode).ThenBy(x => x.ProductoCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<StockProductoResponse>
            {
                Items = paged.Items.Select(CreateStockProductoCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
