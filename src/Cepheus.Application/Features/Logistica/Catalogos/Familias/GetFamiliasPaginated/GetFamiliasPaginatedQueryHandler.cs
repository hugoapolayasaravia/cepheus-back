using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.GetFamiliasPaginated
{
    public class GetFamiliasPaginatedQueryHandler
        : IRequestHandler<GetFamiliasPaginatedQuery, PagedResult<FamiliaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetFamiliasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<FamiliaResponse>> Handle(
            GetFamiliasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.Familias.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(f =>
                    f.Code.ToLower().Contains(search) ||
                    f.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(f => f.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(f => f.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(f => new FamiliaResponse
            {
                Code = f.Code,
                Name = f.Name,
                IsActive = f.IsActive,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                CreatedBy = f.CreatedBy,
                UpdatedBy = f.UpdatedBy,
                RowVersion = f.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}