using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.GetEquiposPaginated
{
    public class GetEquiposPaginatedQueryHandler
        : IRequestHandler<GetEquiposPaginatedQuery, PagedResult<EquipoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetEquiposPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<EquipoResponse>> Handle(
            GetEquiposPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.Equipos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(e =>
                    e.Code.ToLower().Contains(search) ||
                    e.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
            {
                var subCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpper();
                query = query.Where(e => e.SubCentroCostoCode == subCentroCostoCode);
            }

            if (request.Nivel.HasValue)
            {
                query = query.Where(e => e.Nivel == request.Nivel.Value);
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

            var projected = sortedQuery.Select(e => new EquipoResponse
            {
                Code = e.Code,
                Name = e.Name,
                Nivel = e.Nivel,
                SubCentroCostoCode = e.SubCentroCostoCode,
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
