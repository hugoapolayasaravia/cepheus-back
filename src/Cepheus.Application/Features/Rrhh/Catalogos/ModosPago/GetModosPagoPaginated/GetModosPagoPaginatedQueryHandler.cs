using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.GetModosPagoPaginated
{
    public class GetModosPagoPaginatedQueryHandler
        : IRequestHandler<GetModosPagoPaginatedQuery, PagedResult<ModoPagoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetModosPagoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ModoPagoResponse>> Handle(
            GetModosPagoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.ModosPago.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(m =>
                    m.Code.ToLower().Contains(search) ||
                    m.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(m => m.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(m => m.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(m => new ModoPagoResponse
            {
                Code = m.Code,
                Name = m.Name,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedBy = m.UpdatedBy,
                RowVersion = m.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}