using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GenerarOrdenCompra
{
    /// <summary>
    /// Genera una Orden de Compra con los precios del proveedor SELECCIONADO de la cotización.
    /// Las líneas y los orígenes (pedido/ítem/cantidad) salen de la cotización, de modo que la OC
    /// actualiza CantidadEnCompra de los pedidos de origen igual que una OC creada a mano.
    /// </summary>
    public record GenerarOrdenCompraDesdeCotizacionCommand(
        string PlantaCode,
        string CotizacionCode,
        string ProveedorCode,
        string TipoCompraCode,
        string ComprobantePagoCode,
        DateTime FechaEntrega,
        string CompradorCode,
        string LugarEnvioCode,
        string FormaPagoCode,
        string TramiteCode,
        string UnidadNegocioCode,
        string SubCentroCostoCode,
        string? Observaciones1,
        string? Observaciones2,
        string? NotaCompraCode,
        bool EnviarCorreoProveedor
    ) : IRequest<OrdenCompraResponse>;
}
