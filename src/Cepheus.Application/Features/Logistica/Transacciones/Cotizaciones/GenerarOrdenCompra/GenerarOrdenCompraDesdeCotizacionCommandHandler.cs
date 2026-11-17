using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CreateOrdenCompra;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GenerarOrdenCompra
{
    public class GenerarOrdenCompraDesdeCotizacionCommandHandler
        : IRequestHandler<GenerarOrdenCompraDesdeCotizacionCommand, OrdenCompraResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ISender _sender;

        public GenerarOrdenCompraDesdeCotizacionCommandHandler(IUnitOfWork uow, ISender sender)
        {
            _uow = uow;
            _sender = sender;
        }

        public async Task<OrdenCompraResponse> Handle(GenerarOrdenCompraDesdeCotizacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var cotizacionCode = request.CotizacionCode.Trim().ToUpperInvariant();
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .Include(c => c.Proveedores).ThenInclude(p => p.Detalles)
                .Include(c => c.Detalles).ThenInclude(d => d.Origenes)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == cotizacionCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Cotización {plantaCode}/{cotizacionCode} no encontrada.");

            var proveedor = cotizacion.Proveedores.FirstOrDefault(p => p.ProveedorCode == proveedorCode)
                ?? throw new KeyNotFoundException($"El proveedor {proveedorCode} no participa en esta Cotización.");

            if (proveedor.Estado != EstadoProveedorCotizacion.Seleccionado)
            {
                throw new InvalidOperationException(
                    "Solo se puede generar la Orden de Compra del proveedor seleccionado. Seleccione primero al proveedor ganador.");
            }

            var lineas = new List<CreateOrdenCompraDetalleLineaInput>();

            foreach (var oferta in proveedor.Detalles.Where(d => d.CantidadArticulo > 0 && d.PrecioArticulo > 0))
            {
                var lineaCot = cotizacion.Detalles.FirstOrDefault(d => d.ArticuloCode == oferta.ArticuloCode);

                // Orígenes de la cotización repartidos hasta cubrir la cantidad ofertada (la oferta puede ser parcial).
                var origenes = new List<CreateOrdenCompraPedidoOrigenInput>();
                var restante = oferta.CantidadArticulo;
                if (lineaCot is not null)
                {
                    foreach (var o in lineaCot.Origenes.OrderBy(o => o.PedidoCode).ThenBy(o => o.PedidoItemNumber))
                    {
                        if (restante <= 0) break;
                        var tomar = Math.Min(o.CantidadTomada, restante);
                        if (tomar <= 0) continue;
                        origenes.Add(new CreateOrdenCompraPedidoOrigenInput(o.PedidoCode, o.PedidoItemNumber, tomar));
                        restante -= tomar;
                    }
                }

                lineas.Add(new CreateOrdenCompraDetalleLineaInput(
                    oferta.ArticuloCode,
                    oferta.CantidadArticulo,
                    oferta.PrecioArticulo,
                    oferta.DescuentoArticulo,
                    request.SubCentroCostoCode,
                    origenes.Count > 0 ? origenes : null));
            }

            if (lineas.Count == 0)
            {
                throw new InvalidOperationException(
                    "El proveedor seleccionado no tiene líneas con cantidad y precio. Registre su respuesta primero.");
            }

            var comando = new CreateOrdenCompraCommand(
                plantaCode,
                request.TipoCompraCode,
                request.ComprobantePagoCode,
                request.FechaEntrega,
                proveedorCode,
                request.CompradorCode,
                proveedor.MonedaCode,
                request.LugarEnvioCode,
                request.FormaPagoCode,
                request.TramiteCode,
                request.Observaciones1,
                request.Observaciones2,
                request.NotaCompraCode,
                request.UnidadNegocioCode,
                request.EnviarCorreoProveedor,
                0m, 0m, 0m,
                lineas);

            return await _sender.Send(comando, cancellationToken);
        }
    }
}
