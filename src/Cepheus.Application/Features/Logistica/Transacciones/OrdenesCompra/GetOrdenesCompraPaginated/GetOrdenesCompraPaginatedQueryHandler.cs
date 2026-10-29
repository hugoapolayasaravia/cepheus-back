using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.GetOrdenesCompraPaginated
{
    public class GetOrdenesCompraPaginatedQueryHandler
        : IRequestHandler<GetOrdenesCompraPaginatedQuery, PagedResult<OrdenCompraResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetOrdenesCompraPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<OrdenCompraResponse>> Handle(
            GetOrdenesCompraPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<OrdenCompra> query = _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .AsNoTracking()
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Origenes);

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();

                query = query.Where(o =>
                    o.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                // Búsqueda exacta por código.
                var code = request.Code.Trim().ToUpper();

                query = query.Where(o =>
                    o.Code == code);
            }
            else
            {
                if (request.FechaDesde.HasValue)
                {
                    query = query.Where(o =>
                        o.CreatedAt >= request.FechaDesde.Value);
                }

                if (request.FechaHasta.HasValue)
                {
                    query = query.Where(o =>
                        o.CreatedAt <= request.FechaHasta.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.ProveedorCode))
            {
                var proveedorCode = request.ProveedorCode.Trim().ToUpper();

                query = query.Where(o =>
                    o.ProveedorCode == proveedorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoOrdenCompra>(
                    request.Estado,
                    true,
                    out var estado))
            {
                query = query.Where(o =>
                    o.Estado == estado);
            }

            if (!string.IsNullOrWhiteSpace(request.Usuario))
            {
                query = query.Where(o =>
                    o.CreatedBy == request.Usuario);
            }

            if (!string.IsNullOrWhiteSpace(request.FormaPagoCode))
            {
                var formaPagoCode = request.FormaPagoCode.Trim().ToUpper();

                query = query.Where(o =>
                    o.FormaPagoCode == formaPagoCode);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(o => o.CreatedAt)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            return new PagedResult<OrdenCompraResponse>
            {
                Items = pagedEntities.Items
                    .Select(OrdenCompraMapper.Map)
                    .ToList(),

                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}
