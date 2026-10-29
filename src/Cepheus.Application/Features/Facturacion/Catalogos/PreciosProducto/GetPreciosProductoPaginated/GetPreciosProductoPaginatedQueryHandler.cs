using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.GetPreciosProductoPaginated
{
    public class GetPreciosProductoPaginatedQueryHandler
        : IRequestHandler<GetPreciosProductoPaginatedQuery, PagedResult<PrecioProductoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPreciosProductoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PrecioProductoResponse>> Handle(
            GetPreciosProductoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.PreciosProducto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.FleteCode))
            {
                query = query.Where(p => p.FleteCode == request.FleteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoTipoCode))
            {
                query = query.Where(p => p.ProductoTipoCode == request.ProductoTipoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoCode))
            {
                query = query.Where(p => p.ProductoCode == request.ProductoCode);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.FleteCode).ThenBy(p => p.ProductoTipoCode).ThenBy(p => p.ProductoCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(p => new PrecioProductoResponse
            {
                FleteCode = p.FleteCode,
                ProductoTipoCode = p.ProductoTipoCode,
                ProductoCode = p.ProductoCode,
                CurrencyTypeCode = p.CurrencyTypeCode,
                CurrencyCode = p.CurrencyCode,
                Amount = p.Amount,
                TransportAmount = p.TransportAmount,
                FreightAmount = p.FreightAmount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy,
                RowVersion = p.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
