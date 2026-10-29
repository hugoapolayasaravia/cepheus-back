using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.GetSubCentrosEjecutoresPaginated
{
    public class GetSubCentrosEjecutoresPaginatedQueryHandler
        : IRequestHandler<GetSubCentrosEjecutoresPaginatedQuery, PagedResult<SubCentroEjecutorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetSubCentrosEjecutoresPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<SubCentroEjecutorResponse>> Handle(
            GetSubCentrosEjecutoresPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.SubCentrosEjecutores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.Code.ToLower().Contains(search) ||
                    s.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.CentroEjecutorCode))
            {
                var centroEjecutorCode = request.CentroEjecutorCode.Trim().ToUpper();
                query = query.Where(s => s.CentroEjecutorCode == centroEjecutorCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(s => new SubCentroEjecutorResponse
            {
                Code = s.Code,
                Name = s.Name,
                CentroEjecutorCode = s.CentroEjecutorCode,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy,
                RowVersion = s.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
