using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.GetRegimenesLaboralesPaginated
{
    public class GetRegimenesLaboralesPaginatedQueryHandler
        : IRequestHandler<GetRegimenesLaboralesPaginatedQuery, PagedResult<RegimenLaboralResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetRegimenesLaboralesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<RegimenLaboralResponse>> Handle(
            GetRegimenesLaboralesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.RegimenesLaborales.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(r =>
                    r.Code.ToLower().Contains(search) ||
                    r.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(r => r.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(r => new RegimenLaboralResponse
            {
                Code = r.Code,
                Name = r.Name,
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