using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.GetFormasPagoPaginated
{
    public class GetFormasPagoPaginatedQueryHandler
        : IRequestHandler<GetFormasPagoPaginatedQuery, PagedResult<FormaPagoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetFormasPagoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<FormaPagoResponse>> Handle(
            GetFormasPagoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.FormasPago.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(f =>
                    f.Code.ToLower().Contains(search) ||
                    f.Name.ToLower().Contains(search));
            }

            if (request.IsCredit.HasValue)
            {
                query = query.Where(f => f.IsCredit == request.IsCredit.Value);
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

            var projected = sortedQuery.Select(f => new FormaPagoResponse
            {
                Code = f.Code,
                Name = f.Name,
                Days = f.Days,
                IsCredit = f.IsCredit,
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