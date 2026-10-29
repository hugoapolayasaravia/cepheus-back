using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.GetRolesPaginated
{
    public class GetRolesPaginatedQueryHandler
    : IRequestHandler<GetRolesPaginatedQuery, PagedResult<RoleResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetRolesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<RoleResponse>> Handle(
            GetRolesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Administracion.Roles.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(r =>
                    r.Name.ToLower().Contains(search) ||
                    r.Description != null && r.Description.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(r => r.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedBy = r.UpdatedBy,
                RowVersion = r.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }

}
