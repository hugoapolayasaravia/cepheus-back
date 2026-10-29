using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.GetActividadesMantenimientoPaginated
{
    public class GetActividadesMantenimientoPaginatedQueryHandler
        : IRequestHandler<GetActividadesMantenimientoPaginatedQuery, PagedResult<ActividadMantenimientoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetActividadesMantenimientoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ActividadMantenimientoResponse>> Handle(
            GetActividadesMantenimientoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.ActividadCode))
            {
                var act = request.ActividadCode.Trim().ToUpper();
                query = query.Where(a => a.ActividadCode == act);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToUpper();
                query = query.Where(a => a.ActividadCode.Contains(search) || a.ActividadCubiertaCode.Contains(search));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Prioridad).ThenBy(a => a.ActividadCode).ThenBy(a => a.ActividadCubiertaCode)
                : query.ApplySort(request.SortBy, sortDesc);

            return await sortedQuery.Select(ActividadMantenimientoProjection.ToResponse)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
