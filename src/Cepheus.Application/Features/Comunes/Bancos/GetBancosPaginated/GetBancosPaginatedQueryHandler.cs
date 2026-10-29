using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Bancos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Bancos.GetBancosPaginated
{
    public class GetBancosPaginatedQueryHandler
        : IRequestHandler<GetBancosPaginatedQuery, PagedResult<BancoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetBancosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<BancoResponse>> Handle(
            GetBancosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.Bancos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(b =>
                    b.Code.ToLower().Contains(search) ||
                    b.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(b => b.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(b => b.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(b => new BancoResponse
            {
                Code = b.Code,
                Name = b.Name,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                CreatedBy = b.CreatedBy,
                UpdatedBy = b.UpdatedBy,
                RowVersion = b.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
