using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Submodulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Submodulos.GetSubmodulosPaginated
{
    public class GetSubmodulosPaginatedQueryHandler
    : IRequestHandler<GetSubmodulosPaginatedQuery, PagedResult<SubmoduloResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetSubmodulosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<SubmoduloResponse>> Handle(
            GetSubmodulosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Administracion.Submodulos.Query().AsNoTracking();

            if (request.ModuloId.HasValue)
            {
                query = query.Where(s => s.ModuloId == request.ModuloId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.Code.ToLower().Contains(search) ||
                    s.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.Modulo.DisplayOrder).ThenBy(s => s.Modulo.Code).ThenBy(s => s.DisplayOrder).ThenBy(s => s.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(s => new SubmoduloResponse
            {
                Id = s.Id,
                ModuloId = s.ModuloId,
                Code = s.Code,
                Name = s.Name,
                Icon = s.Icon,
                Tooltip = s.Tooltip,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                RowVersion = s.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }


}
