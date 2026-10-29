using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.GetAfpsPaginated
{
    public class GetAfpsPaginatedQueryHandler
        : IRequestHandler<GetAfpsPaginatedQuery, PagedResult<AfpResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAfpsPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AfpResponse>> Handle(
            GetAfpsPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.Afps.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.Code.ToLower().Contains(search) ||
                    a.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(a => new AfpResponse
            {
                Code = a.Code,
                Name = a.Name,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                RowVersion = a.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}