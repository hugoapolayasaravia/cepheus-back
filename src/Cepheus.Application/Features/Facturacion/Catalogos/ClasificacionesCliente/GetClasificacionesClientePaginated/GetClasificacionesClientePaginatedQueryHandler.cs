using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.GetClasificacionesClientePaginated
{
    public class GetClasificacionesClientePaginatedQueryHandler
        : IRequestHandler<GetClasificacionesClientePaginatedQuery, PagedResult<ClasificacionClienteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetClasificacionesClientePaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ClasificacionClienteResponse>> Handle(
            GetClasificacionesClientePaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Catalogos.ClasificacionesCliente.Query().AsNoTracking();

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

            var projected = sortedQuery.Select(t => new ClasificacionClienteResponse
            {
                Code = t.Code,
                Name = t.Name,
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
