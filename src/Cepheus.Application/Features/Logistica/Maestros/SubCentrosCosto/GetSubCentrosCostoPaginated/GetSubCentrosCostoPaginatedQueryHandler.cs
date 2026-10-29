using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.GetSubCentrosCostoPaginated
{
    public class GetSubCentrosCostoPaginatedQueryHandler
        : IRequestHandler<GetSubCentrosCostoPaginatedQuery, PagedResult<SubCentroCostoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetSubCentrosCostoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<SubCentroCostoResponse>> Handle(
            GetSubCentrosCostoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.SubCentrosCosto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.Code.ToLower().Contains(search) ||
                    s.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.CentroCostoCode))
            {
                var centroCostoCode = request.CentroCostoCode.Trim().ToUpper();
                query = query.Where(s => s.CentroCostoCode == centroCostoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(s => s.PlantaCode == plantaCode);
            }

            if (request.ParentCode is not null)
            {
                var parentCode = request.ParentCode.Trim().ToUpper();
                query = parentCode.Length == 0
                    ? query.Where(s => s.ParentCode == null)
                    : query.Where(s => s.ParentCode == parentCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(s => new SubCentroCostoResponse
            {
                Code = s.Code,
                CentroCostoCode = s.CentroCostoCode,
                Name = s.Name,
                AccountingAccountCode = s.AccountingAccountCode,
                AccountingAttachmentTypeCode = s.AccountingAttachmentTypeCode,
                PlantaCode = s.PlantaCode,
                ParentCode = s.ParentCode,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy,
                RowVersion = s.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
