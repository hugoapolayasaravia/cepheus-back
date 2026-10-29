using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.GetVerbosActividadPaginated
{
    public class GetVerbosActividadPaginatedQueryHandler
        : IRequestHandler<GetVerbosActividadPaginatedQuery, PagedResult<VerboActividadResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetVerbosActividadPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<VerboActividadResponse>> Handle(
            GetVerbosActividadPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.VerbosActividad.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(v =>
                    v.Code.ToLower().Contains(search) ||
                    v.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(v => v.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(v => new VerboActividadResponse
            {
                Code = v.Code,
                Name = v.Name,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                CreatedBy = v.CreatedBy,
                UpdatedBy = v.UpdatedBy,
                RowVersion = v.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
