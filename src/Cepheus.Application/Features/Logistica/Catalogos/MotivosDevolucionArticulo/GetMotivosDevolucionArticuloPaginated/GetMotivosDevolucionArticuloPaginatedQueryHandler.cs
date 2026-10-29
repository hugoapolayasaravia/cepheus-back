using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.GetMotivosDevolucionArticuloPaginated;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucion.GetMotivosDevolucionPaginated
{
    public class GetMotivosDevolucionPaginatedQueryHandler
        : IRequestHandler<GetMotivosDevolucionArticuloPaginatedQuery, PagedResult<MotivoDevolucionArticuloResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetMotivosDevolucionPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<MotivoDevolucionArticuloResponse>> Handle(
            GetMotivosDevolucionArticuloPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.MotivosDevolucionArticulo.Query().AsNoTracking();

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

  
            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(m => m.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(m => new MotivoDevolucionArticuloResponse
            {
                Code = m.Code,
                Name = m.Name,
                AffectsStock = m.AffectsStock,
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
