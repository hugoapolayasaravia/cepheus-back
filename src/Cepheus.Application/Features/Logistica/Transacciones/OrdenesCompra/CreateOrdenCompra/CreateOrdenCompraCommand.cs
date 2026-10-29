// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/CreateOrdenCompra/CreateOrdenCompraCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.CreateOrdenCompra
{
    public record CreateOrdenCompraCommand(
        string PlantaCode,
        string TipoCompraCode,
        string ComprobantePagoCode,
        DateTime FechaEntrega,
        string ProveedorCode,
        string CompradorCode,
        string MonedaCode,
        string LugarEnvioCode,
        string FormaPagoCode,
        string TramiteCode,
        string? Observaciones1,
        string? Observaciones2,
        string? NotaCompraCode,
        string UnidadNegocioCode,
        bool EnviarCorreoProveedor,
        decimal NoGravableCompra,
        decimal ServicioCompra,
        decimal IgvExteriorCompra,
        List<CreateOrdenCompraDetalleLineaInput> Detalles
    ) : IRequest<OrdenCompraResponse>;

    public record CreateOrdenCompraDetalleLineaInput(
        string ArticuloCode,
        decimal CantidadArticulo,
        decimal PrecioArticulo,
        decimal DescuentoArticulo,
        string SubCentroCostoCode,
        List<CreateOrdenCompraPedidoOrigenInput>? Origenes
    );

    public record CreateOrdenCompraPedidoOrigenInput(string PedidoCode, int PedidoItemNumber, decimal CantidadTomada);
}