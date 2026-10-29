using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Plantas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Plantas.GetPlantasPaginated
{
    public class GetPlantasPaginatedQueryHandler
        : IRequestHandler<GetPlantasPaginatedQuery, PagedResult<PlantaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPlantasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PlantaResponse>> Handle(
            GetPlantasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.Plantas.Query().AsNoTracking();

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

            if (request.IsProductionPlant.HasValue)
            {
                query = query.Where(p => p.IsProductionPlant == request.IsProductionPlant.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.Name)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(p => new PlantaResponse
            {
                Code = p.Code,
                Name = p.Name,
                LegalName = p.LegalName,
                Address = p.Address,
                AddressComplement = p.AddressComplement,
                UbigeoCode = p.UbigeoCode,
                ManagerName = p.ManagerName,
                HasWarehouse = p.HasWarehouse,
                IsProductionPlant = p.IsProductionPlant,
                IsProject = p.IsProject,
                RequiresApprovals = p.RequiresApprovals,
                AppliesDetraction = p.AppliesDetraction,
                StatusCode = p.StatusCode,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy,
                RowVersion = p.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
