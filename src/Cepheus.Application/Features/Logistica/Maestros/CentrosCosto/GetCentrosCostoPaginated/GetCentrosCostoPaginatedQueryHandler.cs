using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.GetCentrosCostoPaginated
{
    public class GetCentrosCostoPaginatedQueryHandler
        : IRequestHandler<GetCentrosCostoPaginatedQuery, PagedResult<CentroCostoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCentrosCostoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CentroCostoResponse>> Handle(
            GetCentrosCostoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.CentrosCosto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(c => c.PlantaCode == plantaCode);
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

            var projected = sortedQuery.Select(c => new CentroCostoResponse
            {
                Code = c.Code,
                Name = c.Name,
                PlantaCode = c.PlantaCode,
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
