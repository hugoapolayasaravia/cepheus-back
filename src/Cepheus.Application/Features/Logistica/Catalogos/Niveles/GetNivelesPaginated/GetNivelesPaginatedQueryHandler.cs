using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.GetNivelesPaginated
{
    public class GetNivelesPaginatedQueryHandler
        : IRequestHandler<GetNivelesPaginatedQuery, PagedResult<NivelResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetNivelesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<NivelResponse>> Handle(
            GetNivelesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.Niveles.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(n =>
                    n.Code.ToLower().Contains(search) ||
                    n.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(n => n.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(n => n.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(n => new NivelResponse
            {
                Code = n.Code,
                Name = n.Name,
                FechaInicio = n.FechaInicio,
                FechaFin = n.FechaFin,
                IsActive = n.IsActive,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CreatedBy = n.CreatedBy,
                UpdatedBy = n.UpdatedBy,
                RowVersion = n.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
