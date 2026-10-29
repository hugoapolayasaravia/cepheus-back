using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.GetNivelesTrabajadorPaginated
{
    public class GetNivelesTrabajadorPaginatedQueryHandler
        : IRequestHandler<GetNivelesTrabajadorPaginatedQuery, PagedResult<NivelTrabajadorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetNivelesTrabajadorPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<NivelTrabajadorResponse>> Handle(
            GetNivelesTrabajadorPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Catalogos.NivelesTrabajador.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(n =>
                    n.Code.ToLower().Contains(search) ||
                    n.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(n => n.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(n => n.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(n => new NivelTrabajadorResponse
            {
                Code = n.Code,
                Name = n.Name,
                IsActive = n.IsActive,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CreatedBy = n.CreatedBy,
                UpdatedBy = n.UpdatedBy,
                RowVersion = n.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}