using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.GetSubFamiliasPaginated
{
    public class GetSubFamiliasPaginatedQueryHandler
        : IRequestHandler<GetSubFamiliasPaginatedQuery, PagedResult<SubFamiliaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetSubFamiliasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<SubFamiliaResponse>> Handle(
            GetSubFamiliasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.SubFamilias.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.Code.ToLower().Contains(search) ||
                    s.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.FamiliaCode))
            {
                var familiaCode = request.FamiliaCode.Trim().ToUpper();
                query = query.Where(s => s.FamiliaCode == familiaCode);
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

            var projected = sortedQuery.Select(s => new SubFamiliaResponse
            {
                Code = s.Code,
                FamiliaCode = s.FamiliaCode,
                Name = s.Name,
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