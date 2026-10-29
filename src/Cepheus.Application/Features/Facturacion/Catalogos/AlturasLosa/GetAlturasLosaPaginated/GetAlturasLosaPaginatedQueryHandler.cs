using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.GetAlturasLosaPaginated
{
    public class GetAlturasLosaPaginatedQueryHandler
        : IRequestHandler<GetAlturasLosaPaginatedQuery, PagedResult<AlturaLosaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAlturasLosaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AlturaLosaResponse>> Handle(
            GetAlturasLosaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.AlturasLosa.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new AlturaLosaResponse
            {
                Code = t.Code,
                Name = t.Name,
                Value = t.Value,
                Width = t.Width,
                ProductoTipoCode = t.ProductoTipoCode,
                ProductoCode = t.ProductoCode,
                PolystyreneProductoTipoCode = t.PolystyreneProductoTipoCode,
                PolystyreneProductoCode = t.PolystyreneProductoCode,
                PolystyreneValue = t.PolystyreneValue,
                PolystyreneWidth = t.PolystyreneWidth,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
