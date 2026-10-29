using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.GetHorariosPaginated
{
    public class GetHorariosPaginatedQueryHandler
        : IRequestHandler<GetHorariosPaginatedQuery, PagedResult<HorarioResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetHorariosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<HorarioResponse>> Handle(
            GetHorariosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.Horarios.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(h =>
                    h.Code.ToLower().Contains(search) ||
                    h.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(h => h.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(h => h.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(h => new HorarioResponse
            {
                Code = h.Code,
                Name = h.Name,
                IsActive = h.IsActive,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
                CreatedBy = h.CreatedBy,
                UpdatedBy = h.UpdatedBy,
                RowVersion = h.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}