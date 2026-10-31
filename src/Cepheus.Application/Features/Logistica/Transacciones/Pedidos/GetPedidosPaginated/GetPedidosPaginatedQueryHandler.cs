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
        private readonly Cepheus.Application.Comun.Interfaces.ICurrentUserService _current;

        public GetPedidosPaginatedQueryHandler(IUnitOfWork uow, Cepheus.Application.Comun.Interfaces.ICurrentUserService current)
        {
            _uow = uow;
            _current = current;
        }

        public async Task<PagedResult<PedidoResponse>> Handle(
            GetPedidosPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Pedido> query = _uow.Logistica.Transacciones.Pedidos.Query()
                .AsNoTracking()
                .Include(p => p.Detalles);

            // Solo se ven los pedidos de las plantas en las que el usuario está inscrito (UsuariosPlanta).
            var uid = _current.UserId ?? -1;
            var misPlantas = _uow.Logistica.Catalogos.UsuariosPlanta.Query()
                .Where(x => x.UserId == uid).Select(x => x.PlantaCode);
            query = query.Where(p => misPlantas.Contains(p.PlantaCode));

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

            if (!string.IsNullOrWhiteSpace(request.TipoPedidoCode))
            {
                var tipoPedidoCode = request.TipoPedidoCode.Trim().ToUpper();

                query = query.Where(p =>
                    p.TipoPedidoCode == tipoPedidoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.CreatedBy))
            {
                var createdBy = request.CreatedBy.Trim().ToLower();

                query = query.Where(p =>
                    p.CreatedBy != null && p.CreatedBy.ToLower().Contains(createdBy));
            }

            // Fecha de proceso = fecha en que se registró el pedido (auditoría).
            if (request.FechaProcesoDesde.HasValue)
            {
                var desde = request.FechaProcesoDesde.Value.Date;
                query = query.Where(p => p.CreatedAt >= desde);
            }

            if (request.FechaProcesoHasta.HasValue)
            {
                var hasta = request.FechaProcesoHasta.Value.Date.AddDays(1);
                query = query.Where(p => p.CreatedAt < hasta);
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
