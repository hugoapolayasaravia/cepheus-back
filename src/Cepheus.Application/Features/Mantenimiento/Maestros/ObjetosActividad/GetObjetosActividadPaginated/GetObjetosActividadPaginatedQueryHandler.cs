using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.GetObjetosActividadPaginated
{
    public class GetObjetosActividadPaginatedQueryHandler
        : IRequestHandler<GetObjetosActividadPaginatedQuery, PagedResult<ObjetoActividadResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetObjetosActividadPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ObjetoActividadResponse>> Handle(
            GetObjetosActividadPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.ObjetosActividad.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(o =>
                    o.Code.ToLower().Contains(search) ||
                    o.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(o => o.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(o => o.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(o => new ObjetoActividadResponse
            {
                Code = o.Code,
                Name = o.Name,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                CreatedBy = o.CreatedBy,
                UpdatedBy = o.UpdatedBy,
                RowVersion = o.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
