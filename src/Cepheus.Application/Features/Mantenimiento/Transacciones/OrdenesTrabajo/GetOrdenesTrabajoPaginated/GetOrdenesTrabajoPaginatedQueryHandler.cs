using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.GetOrdenesTrabajoPaginated
{
    public class GetOrdenesTrabajoPaginatedQueryHandler
        : IRequestHandler<GetOrdenesTrabajoPaginatedQuery, PagedResult<OrdenTrabajoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOrdenesTrabajoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<OrdenTrabajoResponse>> Handle(
            GetOrdenesTrabajoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(o =>
                    o.Code.ToLower().Contains(search) ||
                    o.Description.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(o => o.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoOrdenTrabajo>(request.Estado, true, out var estado))
            {
                query = query.Where(o => o.Estado == estado);
            }

            if (!string.IsNullOrWhiteSpace(request.EquipoCode))
            {
                var equipoCode = request.EquipoCode.Trim().ToUpper();
                query = query.Where(o => o.EquipoCode == equipoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ResponsableCode))
            {
                var responsableCode = request.ResponsableCode.Trim().ToUpper();
                query = query.Where(o => o.ResponsableCode == responsableCode);
            }

            if (!string.IsNullOrWhiteSpace(request.PrioridadCode))
            {
                var prioridadCode = request.PrioridadCode.Trim().ToUpper();
                query = query.Where(o => o.PrioridadCode == prioridadCode);
            }

            if (request.FechaServicioDesde.HasValue)
            {
                query = query.Where(o => o.FechaProceso >= request.FechaServicioDesde.Value);
            }

            if (request.FechaServicioHasta.HasValue)
            {
                query = query.Where(o => o.FechaProceso <= request.FechaServicioHasta.Value);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(o => o.FechaProceso)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            var items = new List<OrdenTrabajoResponse>(pagedEntities.Items.Count);
            foreach (var o in pagedEntities.Items)
            {
                items.Add(await CreateOrdenTrabajo.CreateOrdenTrabajoCommandHandler.Map(_uow, o, cancellationToken));
            }

            return new PagedResult<OrdenTrabajoResponse>
            {
                Items = items,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}
