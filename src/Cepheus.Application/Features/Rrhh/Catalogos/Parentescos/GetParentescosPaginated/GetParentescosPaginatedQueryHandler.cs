using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.GetParentescosPaginated
{
    public class GetParentescosPaginatedQueryHandler
        : IRequestHandler<GetParentescosPaginatedQuery, PagedResult<ParentescoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetParentescosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ParentescoResponse>> Handle(
            GetParentescosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.Parentescos.Query().AsNoTracking();

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

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(p => new ParentescoResponse
            {
                Code = p.Code,
                Name = p.Name,
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