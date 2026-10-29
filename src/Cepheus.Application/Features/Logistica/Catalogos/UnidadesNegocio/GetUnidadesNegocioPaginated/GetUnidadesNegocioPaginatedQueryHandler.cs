using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.GetUnidadesNegocioPaginated
{
    public class GetUnidadesNegocioPaginatedQueryHandler
        : IRequestHandler<GetUnidadesNegocioPaginatedQuery, PagedResult<UnidadNegocioResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadesNegocioPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UnidadNegocioResponse>> Handle(
            GetUnidadesNegocioPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.UnidadesNegocio.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Code.ToLower().Contains(search) ||
                    (u.Name != null && u.Name.ToLower().Contains(search)));
            }

            // ParentCode vacío ("") filtra unidades raíz (sin padre).
            if (request.ParentCode is not null)
            {
                var parentCode = request.ParentCode.Trim().ToUpper();
                query = parentCode.Length == 0
                    ? query.Where(u => u.ParentCode == null)
                    : query.Where(u => u.ParentCode == parentCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(u => new UnidadNegocioResponse
            {
                Code = u.Code,
                Name = u.Name,
                ParentCode = u.ParentCode,
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