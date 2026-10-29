using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.GetCentrosEjecutoresPaginated
{
    public class GetCentrosEjecutoresPaginatedQueryHandler
        : IRequestHandler<GetCentrosEjecutoresPaginatedQuery, PagedResult<CentroEjecutorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCentrosEjecutoresPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CentroEjecutorResponse>> Handle(
            GetCentrosEjecutoresPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.CentrosEjecutores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(c => new CentroEjecutorResponse
            {
                Code = c.Code,
                Name = c.Name,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                RowVersion = c.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
