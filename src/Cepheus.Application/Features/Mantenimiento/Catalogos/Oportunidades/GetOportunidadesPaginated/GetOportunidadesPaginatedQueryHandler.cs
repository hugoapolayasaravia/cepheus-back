using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.GetOportunidadesPaginated
{
    public class GetOportunidadesPaginatedQueryHandler
        : IRequestHandler<GetOportunidadesPaginatedQuery, PagedResult<OportunidadResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOportunidadesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<OportunidadResponse>> Handle(
            GetOportunidadesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Catalogos.Oportunidades.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(o =>
                    o.Code.ToLower().Contains(search) ||
                    o.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(o => o.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(o => o.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(o => new OportunidadResponse
            {
                Code = o.Code,
                Name = o.Name,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                CreatedBy = o.CreatedBy,
                UpdatedBy = o.UpdatedBy,
                RowVersion = o.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
