using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CreateOrdenCompra
{
    public class CreateOrdenCompraCommandHandler : IRequestHandler<CreateOrdenCompraCommand, OrdenCompraResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CorrelativeLength = 5;
        private const string CodePrefix = "5";

        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public CreateOrdenCompraCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
        }

        public async Task<OrdenCompraResponse> Handle(CreateOrdenCompraCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            var compradorCode = request.CompradorCode.Trim().ToUpperInvariant();

            // Login del usuario autenticado.
            // NO utilizar CompradorCode para CompradoPor.
            var usuarioLogin = _currentUserService.FullName;

            for (var attempt = 1;  attempt <= MaxConcurrencyRetries;  attempt++)
            {
                var nextCode = await NextCodeAsync( plantaCode, cancellationToken);

                var orden = new OrdenCompra
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    TipoCompraCode = request.TipoCompraCode.Trim().ToUpperInvariant(),
                    ComprobantePagoCode = request.ComprobantePagoCode,
                    FechaEntrega = request.FechaEntrega,
                    ProveedorCode = proveedorCode,
                    CompradorCode =  compradorCode,
                    MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                    LugarEnvioCode = request.LugarEnvioCode.Trim().ToUpperInvariant(),
                    FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant(),
                    TramiteCode = request.TramiteCode.Trim().ToUpperInvariant(),
                    Observaciones1 = request.Observaciones1?.Trim(),
                    Observaciones2 = request.Observaciones2?.Trim(),
                    NotaCompraCode =
                        string.IsNullOrWhiteSpace(request.NotaCompraCode)
                            ? null
                            : request.NotaCompraCode.Trim().ToUpperInvariant(),
                    UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant(),
                    EnviarCorreoProveedor = request.EnviarCorreoProveedor,
                    Estado = EstadoOrdenCompra.Pendiente,
                    NoGravableCompra = request.NoGravableCompra,
                    ServicioCompra = request.ServicioCompra,
                    IgvExteriorCompra = request.IgvExteriorCompra
                };

                var item = 1;

                // Acumula cuánto se toma de cada línea de Pedido
                // dentro de esta Orden de Compra.
                var cantidadesTomadas =
                    new Dictionary<
                        (string PlantaCode,
                         string PedidoCode,
                         int ItemNumber),
                        decimal>();

                foreach (var line in request.Detalles)
                {
                    var articuloCode = line.ArticuloCode.Trim().ToUpperInvariant();

                    var detalle = new OrdenCompraDetalle
                    {
                        PlantaCode = plantaCode,
                        OrdenCompraCode = nextCode,
                        ArticuloCode = articuloCode,
                        ItemNumber = item++,

                        CantidadArticulo = line.CantidadArticulo,
                        PrecioArticulo = line.PrecioArticulo,
                        DescuentoArticulo = line.DescuentoArticulo,

                        TotalArticulo =
                            OrdenCompraTotalsCalculator.CalculateLineTotal(
                                line.CantidadArticulo,
                                line.PrecioArticulo,
                                line.DescuentoArticulo),

                        SubCentroCostoCode =
                            line.SubCentroCostoCode
                                .Trim()
                                .ToUpperInvariant()
                    };

                    // Una OC puede crearse sin Pedido.
                    if (line.Origenes is { Count: > 0 })
                    {
                        foreach (var origen in line.Origenes)
                        {
                            var pedidoCode = origen.PedidoCode
                                .Trim()
                                .ToUpperInvariant();

                            detalle.Origenes.Add(
                                new OrdenCompraPedidoOrigen
                                {
                                    PlantaCode = plantaCode,
                                    OrdenCompraCode = nextCode,
                                    ArticuloCode = articuloCode,
                                    PedidoCode = pedidoCode,
                                    PedidoItemNumber =
                                        origen.PedidoItemNumber,
                                    CantidadTomada =
                                        origen.CantidadTomada,
                                    CantidadEntregada = 0
                                });

                            var key =
                                (
                                    plantaCode,
                                    pedidoCode,
                                    origen.PedidoItemNumber
                                );

                            if (cantidadesTomadas.ContainsKey(key))
                            {
                                cantidadesTomadas[key] +=
                                    origen.CantidadTomada;
                            }
                            else
                            {
                                cantidadesTomadas[key] =
                                    origen.CantidadTomada;
                            }
                        }
                    }

                    orden.Detalles.Add(detalle);
                }

                await OrdenCompraTotalsCalculator.RecalculateAsync(
                    _uow,
                    orden,
                    cancellationToken);

                await _uow.Logistica.Transacciones.OrdenesCompra
                    .AddAsync(
                        orden,
                        cancellationToken);

                // =====================================================
                // ACTUALIZAR PEDIDOS
                // =====================================================

                // Guardamos los Pedidos afectados para actualizar
                // posteriormente el estado de la cabecera.
                var pedidosAfectados =
                    new HashSet<(string PlantaCode, string PedidoCode)>();

                foreach (var itemPedido in cantidadesTomadas)
                {
                    var pedidoDetalle =
                        await _uow.Logistica.Transacciones.PedidoDetalles
                            .Query()
                            .Include(pd => pd.Pedido)
                            .FirstOrDefaultAsync(
                                pd =>
                                    pd.PlantaCode ==
                                        itemPedido.Key.PlantaCode &&
                                    pd.PedidoCode ==
                                        itemPedido.Key.PedidoCode &&
                                    pd.ItemNumber ==
                                        itemPedido.Key.ItemNumber,
                                cancellationToken);

                    if (pedidoDetalle is null)
                        continue;

                    // =================================================
                    // CANTIDAD TOMADA POR OTRAS OC
                    // =================================================

                    var cantidadAnterior =
                        await _uow.Logistica.Transacciones
                            .OrdenCompraPedidoOrigenes
                            .Query()
                            .Where(o =>
                                o.PlantaCode ==
                                    itemPedido.Key.PlantaCode &&
                                o.PedidoCode ==
                                    itemPedido.Key.PedidoCode &&
                                o.PedidoItemNumber ==
                                    itemPedido.Key.ItemNumber)
                            .SumAsync(
                                o => o.CantidadTomada,
                                cancellationToken);

                    var cantidadNueva =
                        itemPedido.Value;

                    // =================================================
                    // ACTUALIZAR CANTIDAD EN COMPRA
                    // =================================================

                    pedidoDetalle.CantidadEnCompra =
                        cantidadAnterior + cantidadNueva;

                    // Última OC que tomó esta línea.
                    pedidoDetalle.OrdenCompraCode =
                        nextCode;

                    // Proveedor de la última OC.
                    pedidoDetalle.ProveedorCode =
                        proveedorCode;

                    // =================================================
                    // ESTADO DEL DETALLE DEL PEDIDO
                    // =================================================

                    if (pedidoDetalle.CantidadEnCompra >=
                        pedidoDetalle.CantidadArticulo)
                    {
                        pedidoDetalle.EstadoPedidoDetalle =
                            EstadoPedidoDetalle.EnCompra;
                    }
                    else
                    {
                        pedidoDetalle.EstadoPedidoDetalle =
                            EstadoPedidoDetalle.CompraParcial;
                    }

                    // =================================================
                    // DATOS DE COMPRA DE LA CABECERA
                    // =================================================

                    if (pedidoDetalle.Pedido is not null)
                    {
                        pedidoDetalle.Pedido.CompradoPor =
                            usuarioLogin;

                        pedidoDetalle.Pedido.FechaCompra =
                            DateTime.Now;

                        pedidosAfectados.Add(
                            (
                                pedidoDetalle.PlantaCode,
                                pedidoDetalle.PedidoCode
                            ));
                    }
                }

                // =====================================================
                // ACTUALIZAR ESTADO DE LA CABECERA DEL PEDIDO
                // =====================================================

                foreach (var pedidoKey in pedidosAfectados)
                {
                    var pedido =
                        await _uow.Logistica.Transacciones.Pedidos
                            .Query()
                            .Include(p => p.Detalles)
                            .FirstOrDefaultAsync(
                                p =>
                                    p.PlantaCode ==
                                        pedidoKey.PlantaCode &&
                                    p.Code ==
                                        pedidoKey.PedidoCode,
                                cancellationToken);

                    if (pedido is null)
                        continue;

                    // Si TODOS los detalles están completamente
                    // tomados por órdenes de compra:
                    //
                    //     10 / 10 -> EnCompra (16)
                    //     20 / 20 -> EnCompra (16)
                    //
                    // entonces la cabecera pasa a EnCompra (16).
                    //
                    // Si al menos un detalle todavía tiene cantidad
                    // pendiente:
                    //
                    //     10 / 10 -> EnCompra (16)
                    //     20 /  5 -> CompraParcial (17)
                    //
                    // la cabecera pasa a CompraParcial (17).

                    var compraCompleta =
                        pedido.Detalles.Count > 0 &&
                        pedido.Detalles.All(
                            d =>
                                d.CantidadEnCompra >=
                                d.CantidadArticulo);

                    pedido.EstadoPedido =
                        compraCompleta
                            ? EstadoPedido.EnCompra
                            : EstadoPedido.CompraParcial;
                }

                try
                {
                    await _uow.SaveChangesAsync(
                        cancellationToken);

                    return OrdenCompraMapper.Map(orden);
                }
                catch (DbUpdateException)
                    when (attempt < MaxConcurrencyRetries)
                {
                    _uow.ClearTracking();
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Orden de Compra " +
                $"para la planta {plantaCode} por alta concurrencia. " +
                $"Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(
            string plantaCode,
            CancellationToken cancellationToken)
        {
            var lastCode =
                await _uow.Logistica.Transacciones.OrdenesCompra
                    .Query()
                    .AsNoTracking()
                    .Where(o =>
                        o.PlantaCode == plantaCode &&
                        o.Code.StartsWith(CodePrefix))
                    .OrderByDescending(o => o.Code)
                    .Select(o => o.Code)
                    .FirstOrDefaultAsync(
                        cancellationToken);

            var next = 1;

            if (lastCode is not null &&
                int.TryParse(
                    lastCode.Substring(CodePrefix.Length),
                    out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return CodePrefix +
                   next.ToString()
                       .PadLeft(
                           CorrelativeLength,
                           '0');
        }
    }
}