using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresAsignadosPaginated
{
    public class GetAprobadoresAsignadosPaginatedQueryHandler
        : IRequestHandler<GetAprobadoresAsignadosPaginatedQuery, PagedResult<AprobadorAsignadoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAprobadoresAsignadosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<AprobadorAsignadoResponse>> Handle(
            GetAprobadoresAsignadosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.AprobadoresAsignados.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TrabajadorCode))
            {
                var t = request.TrabajadorCode.Trim().ToUpper();
                query = query.Where(a => a.TrabajadorCode == t ||
                                          a.SuplenteTrabajadorCode == t ||
                                          a.SuperiorTrabajadorCode == t);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoTransaccionCode))
            {
                var tt = request.TipoTransaccionCode.Trim().ToUpper();
                query = query.Where(a => a.TipoTransaccionCode == tt);
            }

            if (!string.IsNullOrWhiteSpace(request.UnidadNegocioCode))
            {
                var u = request.UnidadNegocioCode.Trim().ToUpper();
                query = query.Where(a => a.UnidadNegocioCode == u);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.UnidadNegocioCode).ThenBy(a => a.TipoTransaccionCode).ThenBy(a => a.NivelCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(a => new AprobadorAsignadoResponse
            {
                NivelCode = a.NivelCode,
                TipoTransaccionCode = a.TipoTransaccionCode,
                UnidadNegocioCode = a.UnidadNegocioCode,
                MonedaCode = a.MonedaCode,
                TrabajadorCode = a.TrabajadorCode,
                SuplenteTrabajadorCode = a.SuplenteTrabajadorCode,
                SuperiorTrabajadorCode = a.SuperiorTrabajadorCode,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                RowVersion = a.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
