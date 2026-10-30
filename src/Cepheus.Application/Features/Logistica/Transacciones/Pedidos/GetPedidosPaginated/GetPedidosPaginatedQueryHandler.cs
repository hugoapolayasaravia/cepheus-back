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
            IQueryable<Pedido> query = _uow.Logistica.Transacciones.Pedidos
                .Query()
                .AsNoTracking();

            // Planta: PR.Codigo_Pla
            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = request.PlantaCode.Trim().ToUpper();
                query = query.Where(p => p.PlantaCode == planta);
            }

            // Código de pedido: PR.Codigo_Ped
            // Al buscar un código específico, el SP omite los demás filtros.
            if (!string.IsNullOrWhiteSpace(request.Search)
                && !request.Search.Trim().Equals(
                    "T", StringComparison.OrdinalIgnoreCase))
            {
                var codigoPedido = request.Search.Trim();
                query = query.Where(p => p.Code == codigoPedido);
            }
            else
            {
                // Estado: PR.Codigo_Est
                if (!string.IsNullOrWhiteSpace(request.Estado)
                    && !request.Estado.Trim().Equals(
                        "T", StringComparison.OrdinalIgnoreCase))
                {
                    var estadoTexto = request.Estado.Trim();

                    if (Enum.TryParse<EstadoPedido>(
                        estadoTexto,
                        true,
                        out var estado))
                    {
                        query = query.Where(p => p.EstadoPedido == estado);
                    }
                    else
                    {
                        query = query.Where(p => false);
                    }
                }

                // Subcentro de costo: PR.Codigo_Scc
                if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode)
                    && !request.SubCentroCostoCode.Trim().Equals(
                        "T", StringComparison.OrdinalIgnoreCase))
                {
                    var subCentroCosto = request.SubCentroCostoCode.Trim();
                    query = query.Where(p =>
                        p.SubCentroCostoCode == subCentroCosto);
                }

                // Usuario: PR.Usuario
                if (!string.IsNullOrWhiteSpace(request.UsuarioCode)
                    && !request.UsuarioCode.Trim().Equals(
                        "T", StringComparison.OrdinalIgnoreCase))
                {
                    var usuario = request.UsuarioCode.Trim();
                    query = query.Where(p => p.CreatedBy == usuario);
                }

                // Tipo de pedido: TP.Codigo_Tpe
                if (!string.IsNullOrWhiteSpace(request.TipoPedidoCode)
                    && !request.TipoPedidoCode.Trim().Equals(
                        "T", StringComparison.OrdinalIgnoreCase))
                {
                    var tipoPedido = request.TipoPedidoCode.Trim();
                    query = query.Where(p =>
                        p.TipoPedidoCode == tipoPedido);
                }

                // Fecha inicial: PR.Fecha_Pro.
                // Se conserva FechaEntrega porque es la propiedad conocida
                // en la entidad compartida en esta conversación.
                if (request.FechaPedidoDesde.HasValue)
                {
                    var fechaDesde = request.FechaPedidoDesde.Value.Date;

                    query = query.Where(p =>
                        p.FechaEntrega >= fechaDesde);
                }

                // Fecha final inclusiva.
                if (request.FechaPedidoHasta.HasValue)
                {
                    var fechaHastaExclusiva =
                        request.FechaPedidoHasta.Value.Date.AddDays(1);

                    query = query.Where(p =>
                        p.FechaEntrega < fechaHastaExclusiva);
                }
            }

            // Paginación
            var pageNumber = Math.Max(request.PageNumber ?? 1, 1);
            var pageSize = Math.Clamp(request.PageSize ?? 10, 1, 100);
            var sortDesc = request.SortDesc ?? true;

            IQueryable<Pedido> sortedQuery;

            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                sortedQuery = query
                    .OrderByDescending(p => p.CreatedAt)
                    .ThenByDescending(p => p.Code);
            }
            else
            {
                sortedQuery = query.ApplySort(request.SortBy, sortDesc);
            }

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
