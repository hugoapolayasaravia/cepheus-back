using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.GetMotivosDevolucionPaginated
{
    public class GetMotivosDevolucionPaginatedQueryHandler
        : IRequestHandler<GetMotivosDevolucionPaginatedQuery, PagedResult<MotivoDevolucionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivosDevolucionPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<MotivoDevolucionResponse>> Handle(
            GetMotivosDevolucionPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.MotivosDevolucion.Query().AsNoTracking();

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

            if (request.AffectsStock.HasValue)
            {
                query = query.Where(m => m.AffectsStock == request.AffectsStock.Value);
            }

            if (request.EsVenta.HasValue)
            {
                query = query.Where(m => m.EsVenta == request.EsVenta.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(m => m.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(m => new MotivoDevolucionResponse
            {
                Code = m.Code,
                Name = m.Name,
                AffectsStock = m.AffectsStock,
                EsVenta = m.EsVenta,
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
