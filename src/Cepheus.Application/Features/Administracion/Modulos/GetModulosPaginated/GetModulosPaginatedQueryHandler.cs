using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Modulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Modulos.GetModulosPaginated
{
    public class GetModulosPaginatedQueryHandler
    : IRequestHandler<GetModulosPaginatedQuery, PagedResult<ModuloResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetModulosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ModuloResponse>> Handle(
            GetModulosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Administracion.Modulos.Query().AsNoTracking();

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

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(m => new ModuloResponse
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Icon = m.Icon,
                Tooltip = m.Tooltip,
                DisplayOrder = m.DisplayOrder,
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
