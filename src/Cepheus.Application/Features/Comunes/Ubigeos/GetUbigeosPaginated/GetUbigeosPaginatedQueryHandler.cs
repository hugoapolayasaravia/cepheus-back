using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Ubigeos.GetUbigeosPaginated
{
    public class GetUbigeosPaginatedQueryHandler
        : IRequestHandler<GetUbigeosPaginatedQuery, PagedResult<UbigeoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUbigeosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UbigeoResponse>> Handle(
            GetUbigeosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Comunes.Ubigeos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Code.ToLower().Contains(search) ||
                    u.Department.ToLower().Contains(search) ||
                    u.Province.ToLower().Contains(search) ||
                    u.District.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Department))
            {
                var department = request.Department.Trim().ToLower();
                query = query.Where(u => u.Department.ToLower() == department);
            }

            if (!string.IsNullOrWhiteSpace(request.Province))
            {
                var province = request.Province.Trim().ToLower();
                query = query.Where(u => u.Province.ToLower() == province);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.Department).ThenBy(u => u.Province).ThenBy(u => u.District)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(u => new UbigeoResponse
            {
                Code = u.Code,
                Department = u.Department,
                Province = u.Province,
                District = u.District,
                FullAddress = u.FullAddress,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                CreatedBy = u.CreatedBy,
                UpdatedBy = u.UpdatedBy,
                RowVersion = u.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
