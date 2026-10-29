using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidosPaginated
{
    public class GetPedidosPaginatedQueryHandler
        : IRequestHandler<GetPedidosPaginatedQuery, PagedResult<PedidoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPedidosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PedidoResponse>> Handle(
            GetPedidosPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Pedido> query = _uow.Logistica.Transacciones.Pedidos.Query()
                .AsNoTracking()
                .Include(p => p.Detalles);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();

                query = query.Where(p =>
                    p.Code.ToLower().Contains(search) ||
                    p.Observaciones.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();

                query = query.Where(p =>
                    p.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoPedido>(
                    request.Estado,
                    true,
                    out var estado))
            {
                query = query.Where(p =>
                    p.EstadoPedido == estado);
            }

            if (!string.IsNullOrWhiteSpace(request.TrabajadorCode))
            {
                var trabajadorCode = request.TrabajadorCode.Trim().ToUpper();

                query = query.Where(p =>
                    p.TrabajadorCode == trabajadorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
            {
                var subCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpper();

                query = query.Where(p =>
                    p.SubCentroCostoCode == subCentroCostoCode);
            }

            if (request.FechaEntregaDesde.HasValue)
            {
                query = query.Where(p =>
                    p.FechaEntrega >= request.FechaEntregaDesde.Value);
            }

            if (request.FechaEntregaHasta.HasValue)
            {
                query = query.Where(p =>
                    p.FechaEntrega <= request.FechaEntregaHasta.Value);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.ApplySort(request.SortBy, sortDesc);

            var pagedEntities = await sortedQuery.ToPagedResultAsync(
                pageNumber,
                pageSize,
                cancellationToken);

            return new PagedResult<PedidoResponse>
            {
                Items = pagedEntities.Items
                    .Select(CreatePedidoCommandHandler.Map)
                    .ToList(),

                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}
