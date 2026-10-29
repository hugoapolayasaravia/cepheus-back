using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.GetEstadosTrabajadorPaginated
{
    public class GetEstadosTrabajadorPaginatedQueryHandler
        : IRequestHandler<GetEstadosTrabajadorPaginatedQuery, PagedResult<EstadoTrabajadorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetEstadosTrabajadorPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<EstadoTrabajadorResponse>> Handle(
            GetEstadosTrabajadorPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.EstadosTrabajador.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(e =>
                    e.Code.ToLower().Contains(search) ||
                    e.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(e => e.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(e => new EstadoTrabajadorResponse
            {
                Code = e.Code,
                Name = e.Name,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedBy = e.CreatedBy,
                UpdatedBy = e.UpdatedBy,
                RowVersion = e.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}