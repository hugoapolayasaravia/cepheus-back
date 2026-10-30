
namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Impresion;

public sealed class PedidoImpresionResponse
{
    public string PlantaCode { get; set; } = string.Empty;
    public string PedidoCode { get; set; } = string.Empty;

    public DateTime FechaPro { get; set; }

    public string OrigenPedido { get; set; } = string.Empty;
    public string CodigoCosto { get; set; } = string.Empty;

    public string? CodigoOrdenTrabajo { get; set; }
    public string? DescripcionOrdenTrabajo { get; set; }

    public string? CodigoSubCentroCosto { get; set; }
    public string? DescripcionSubCentroCosto { get; set; }

    public string? Observaciones { get; set; }

    public string? CodigoTrabajadorSolicitante { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;

    public string? AprobadoPor { get; set; }
    public string NombreAprobador { get; set; } = string.Empty;

    public string? DescripcionPlanta { get; set; }

    public List<PedidoImpresionDetalleResponse> Detalles { get; set; } = new();
}

public sealed class PedidoImpresionDetalleResponse
{
    public string? ArticuloCode { get; set; }
    public int ItemNumber { get; set; }
    public string DescripcionArticulo { get; set; } = string.Empty;
    public decimal CantidadArticulo { get; set; }
    public string UnidadMedidaCode { get; set; } = string.Empty;
}
