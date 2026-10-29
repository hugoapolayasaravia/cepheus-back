// Cepheus.Application/Features/Logistica/Transacciones/OrdenCompraDetalles/CreateOrdenCompraDetalle/CreateOrdenCompraDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenCompraDetalles.CreateOrdenCompraDetalle
{
    public class CreateOrdenCompraDetalleCommandHandler : IRequestHandler<CreateOrdenCompraDetalleCommand, OrdenCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOrdenCompraDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraResponse> Handle(CreateOrdenCompraDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenCompraCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == ordenCode, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra {plantaCode}/{ordenCode} no encontrada.");
            }

            if (orden.Estado != EstadoOrdenCompra.Pendiente)
            {
                throw new InvalidOperationException($"La Orden de Compra está en estado '{orden.Estado}' y ya no admite agregar líneas.");
            }

            if (orden.Detalles.Any(d => d.ArticuloCode == articuloCode))
            {
                throw new InvalidOperationException($"El artículo {articuloCode} ya está en esta Orden de Compra.");
            }

            var nextItem = orden.Detalles.Count == 0 ? 1 : orden.Detalles.Max(d => d.ItemNumber) + 1;

            var detalle = new OrdenCompraDetalle
            {
                PlantaCode = plantaCode,
                OrdenCompraCode = ordenCode,
                ArticuloCode = articuloCode,
                ItemNumber = nextItem,
                CantidadArticulo = request.CantidadArticulo,
                PrecioArticulo = request.PrecioArticulo,
                DescuentoArticulo = request.DescuentoArticulo,
                TotalArticulo = OrdenCompraTotalsCalculator.CalculateLineTotal(request.CantidadArticulo, request.PrecioArticulo, request.DescuentoArticulo),
                SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant()
            };

            var pedidoLineasAfectadas = new List<(string, string, int)>();
            if (request.Origenes is { Count: > 0 })
            {
                foreach (var origen in request.Origenes)
                {
                    var pedidoCode = origen.PedidoCode.Trim().ToUpperInvariant();
                    detalle.Origenes.Add(new OrdenCompraPedidoOrigen
                    {
                        PlantaCode = plantaCode,
                        OrdenCompraCode = ordenCode,
                        ArticuloCode = articuloCode,
                        PedidoCode = pedidoCode,
                        PedidoItemNumber = origen.PedidoItemNumber,
                        CantidadTomada = origen.CantidadTomada
                    });
                    pedidoLineasAfectadas.Add((plantaCode, pedidoCode, origen.PedidoItemNumber));
                }
            }

            orden.Detalles.Add(detalle);
            await _uow.Logistica.Transacciones.OrdenCompraDetalles.AddAsync(detalle, cancellationToken);

            await OrdenCompraTotalsCalculator.RecalculateAsync(_uow, orden, cancellationToken);

            if (pedidoLineasAfectadas.Count > 0)
            {
                await OrdenCompraTotalsCalculator.RecalculatePedidoCantidadEnCompraAsync(_uow, pedidoLineasAfectadas, cancellationToken);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraMapper.Map(orden);
        }
    }
}