using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.GetPedidoDetalles;

public sealed class GetPedidoDetallesQueryHandler
    : IRequestHandler<GetPedidoDetallesQuery, IReadOnlyList<PedidoDetalleResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetPedidoDetallesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<PedidoDetalleResponse>> Handle(
        GetPedidoDetallesQuery request,
        CancellationToken cancellationToken)
    {
        var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
        var pedidoCode = request.PedidoCode.Trim().ToUpperInvariant();

        // Obtener las líneas del pedido ordenadas por número de ítem.
        var detalles = await _uow.Logistica.Transacciones.PedidoDetalles
            .Query()
            .AsNoTracking()
            .Where(d =>
                d.PlantaCode == plantaCode &&
                d.PedidoCode == pedidoCode)
            .OrderBy(d => d.ItemNumber)
            .Select(d => new
            {
                d.PlantaCode,
                d.PedidoCode,
                d.ArticuloCode,
                d.DescripcionArticulo,
                d.UnidadMedidaCode,
                d.PrecioArticulo,
                d.CantidadArticulo,
                d.TotalArticulo,
                d.ItemNumber,
                d.EstadoPedidoDetalle,
                d.OrdenCompraCode,
                NombreProveedor = d.Proveedor == null
                    ? null
                    : !string.IsNullOrWhiteSpace(d.Proveedor.TradeName)
                        ? d.Proveedor.TradeName
                        : d.Proveedor.LegalName
            })
            .ToListAsync(cancellationToken);

        var resultado = new List<PedidoDetalleResponse>(detalles.Count);

        foreach (var detalle in detalles)
        {
            decimal pendiente = 0m;

            // Las líneas libres, sin código de artículo,
            // no tienen stock ni cantidades reservadas.
            if (!string.IsNullOrWhiteSpace(detalle.ArticuloCode))
            {
                var articuloCode = detalle.ArticuloCode.Trim();

                // Equivale a fn_Stock_Articulo del sistema legacy.
                var stockReal = await _uow.Logistica.Maestros.StockArticulos
                    .Query()
                    .AsNoTracking()
                    .Where(s =>
                        s.PlantaCode == plantaCode &&
                        s.ArticuloCode == articuloCode)
                    .Select(s => (decimal?)s.Quantity)
                    .FirstOrDefaultAsync(cancellationToken) ?? 0m;

                // Equivale al primer bloque de fn_Stock_Pendiente:
                // vales con detalle en estado '01'.
                var cantidadValesPendientes =
                    await _uow.Logistica.Transacciones.ValeDetalles
                        .Query()
                        .AsNoTracking()
                        .Where(d =>
                            d.PlantaCode == plantaCode &&
                            d.ArticuloCode == articuloCode &&
                            d.Estado == EstadoValeDetalle.Pendiente)
                        .SumAsync(d => d.Cantidad, cancellationToken);

                // Equivale al segundo bloque de fn_Stock_Pendiente:
                // ajustes de inventario con detalle en estado '01'.
                var cantidadAjustesPendientes =
                    await _uow.Logistica.Transacciones.AjusteInventarioDetalles
                        .Query()
                        .AsNoTracking()
                        .Where(d =>
                            d.PlantaCode == plantaCode &&
                            d.ArticuloCode == articuloCode &&
                            d.Estado == EstadoAjusteInventarioDetalle.Pendiente)
                        .SumAsync(d => d.Cantidad, cancellationToken);

                pendiente = stockReal
                    - cantidadValesPendientes
                    - cantidadAjustesPendientes;
            }

            resultado.Add(new PedidoDetalleResponse(
                PlantaCode: detalle.PlantaCode,
                PedidoCode: detalle.PedidoCode,
                ArticuloCode: detalle.ArticuloCode,
                DescripcionArticulo: detalle.DescripcionArticulo,
                UnidadMedidaCode: detalle.UnidadMedidaCode,
                PrecioArticulo: detalle.PrecioArticulo,
                CantidadArticulo: detalle.CantidadArticulo,
                TotalArticulo: detalle.TotalArticulo,
                ItemNumber: detalle.ItemNumber,
                CodigoEstado: MapearCodigoEstado(detalle.EstadoPedidoDetalle),
                OrdenCompraCode: detalle.OrdenCompraCode,
                NombreProveedor: detalle.NombreProveedor,
                Stock: 0m,
                Pendiente: pendiente
            ));
        }

        return resultado;
    }

    private static string MapearCodigoEstado(EstadoPedidoDetalle estado)
    {
        return estado switch
        {
            EstadoPedidoDetalle.Pendiente => "01",
            EstadoPedidoDetalle.Anulado => "04",
            EstadoPedidoDetalle.EnCompra => "16",
            EstadoPedidoDetalle.CompraParcial => "17",
            _ => ((int)estado).ToString("D2")
        };
    }
}