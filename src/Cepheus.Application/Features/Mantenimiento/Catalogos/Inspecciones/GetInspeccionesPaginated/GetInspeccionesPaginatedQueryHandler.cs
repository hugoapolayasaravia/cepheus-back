using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.GetInspeccionesPaginated
{
    public class GetInspeccionesPaginatedQueryHandler
        : IRequestHandler<GetInspeccionesPaginatedQuery, PagedResult<InspeccionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetInspeccionesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<InspeccionResponse>> Handle(
            GetInspeccionesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Catalogos.Inspecciones.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(i =>
                    i.Code.ToLower().Contains(search) ||
                    i.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(i => i.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(i => i.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(i => new InspeccionResponse
            {
                Code = i.Code,
                Name = i.Name,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                CreatedBy = i.CreatedBy,
                UpdatedBy = i.UpdatedBy,
                RowVersion = i.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
