// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/GetOrdenesCompraAdministracionPaginated/GetOrdenesCompraAdministracionPaginatedQueryHandler.cs
using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.GetOrdenesCompraAdmPaginated
{
    public class GetOrdenesCompraAdmPaginatedQueryHandler
        : IRequestHandler<GetOrdenesCompraAdmPaginatedQuery, PagedResult<OrdenCompraAdmListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOrdenesCompraAdmPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<OrdenCompraAdmListItemResponse>> Handle(
            GetOrdenesCompraAdmPaginatedQuery request, CancellationToken cancellationToken)
        {
            IQueryable<OrdenCompraAdministracion> query = _uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                .AsNoTracking()
                .Include(o => o.Proveedor);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                // Búsqueda exacta por código (el rango de fechas solo aplica sin código, como en el legacy).
                var code = request.Code.Trim().ToUpper();
                query = query.Where(o => o.Code == code);
            }
            else
            {
                if (request.FechaDesde.HasValue)
                {
                    query = query.Where(o => o.CreatedAt >= request.FechaDesde.Value);
                }

                if (request.FechaHasta.HasValue)
                {
                    query = query.Where(o => o.CreatedAt <= request.FechaHasta.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.ProveedorCode))
            {
                var proveedorCode = request.ProveedorCode.Trim().ToUpper();
                query = query.Where(o => o.ProveedorCode == proveedorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoOrdenCompraAdministracion>(request.Estado, true, out var estado))
            {
                query = query.Where(o => o.Estado == estado);
            }

            if (!string.IsNullOrWhiteSpace(request.Usuario))
            {
                query = query.Where(o => o.CreatedBy == request.Usuario);
            }

            if (!string.IsNullOrWhiteSpace(request.FormaPagoCode))
            {
                var formaPagoCode = request.FormaPagoCode.Trim().ToUpper();
                query = query.Where(o => o.FormaPagoCode == formaPagoCode);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            // Orden por defecto del legacy: Fecha_Pro DESC, Codigo_Com DESC.
            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<OrdenCompraAdmListItemResponse>
            {
                Items = pagedEntities.Items
                    .Select(OrdenCompraAdmMapper.MapListItem)
                    .ToList(),
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}
