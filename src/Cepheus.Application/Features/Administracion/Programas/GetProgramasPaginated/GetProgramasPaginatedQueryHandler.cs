using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Programas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Programas.GetProgramasPaginated
{
    public class GetProgramasPaginatedQueryHandler
    : IRequestHandler<GetProgramasPaginatedQuery, PagedResult<ProgramaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProgramasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ProgramaResponse>> Handle(
            GetProgramasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Administracion.Programas.Query().AsNoTracking();

            if (request.SubmoduloId.HasValue)
            {
                query = query.Where(p => p.SubmoduloId == request.SubmoduloId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p =>
                    p.Code.ToLower().Contains(search) ||
                    p.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.Submodulo.Modulo.DisplayOrder).ThenBy(p => p.Submodulo.Modulo.Code).ThenBy(p => p.Submodulo.DisplayOrder).ThenBy(p => p.Submodulo.Code).ThenBy(p => p.DisplayOrder).ThenBy(p => p.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(p => new ProgramaResponse
            {
                Id = p.Id,
                SubmoduloId = p.SubmoduloId,
                Code = p.Code,
                Name = p.Name,
                Icon = p.Icon,
                Tooltip = p.Tooltip,
                Route = p.Route,
                DisplayOrder = p.DisplayOrder,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                RowVersion = p.RowVersion,
                Permissions = p.Permissions
                    .Select(perm => new PermissionSummary { Id = perm.Id, Code = perm.Code, Name = perm.Name })
                    .ToList()
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }


}
