// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/Common/OrdenCompraAdministracionResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common
{
    public class OrdenCompraAdmDetalleResponse
    {
        public int ItemNumber { get; set; }
        public string Glosa { get; set; } = default!;
        public string UnidadMedidaCode { get; set; } = default!;
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string SubCentroCostoCode { get; set; } = default!;
        public decimal CantidadEntregada { get; set; }
    }

    public class OrdenCompraAdmResponse
    {
        public string Code { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public string? EmitirDocumento { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string CompradorCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public string LugarEnvioCode { get; set; } = default!;
        public string FormaPagoCode { get; set; } = default!;
        public string TramiteCode { get; set; } = default!;
        public string? Observaciones1 { get; set; }
        public string? Observaciones2 { get; set; }
        public string? NotaCompraCode { get; set; }
        public string UnidadNegocioCode { get; set; } = default!;
        public string Motivo { get; set; } = default!;
        public string Estado { get; set; } = default!;
        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public decimal NetoCompra { get; set; }
        public decimal IgvCompra { get; set; }
        public decimal TotalCompra { get; set; }
        public decimal NoGravableCompra { get; set; }
        public decimal RentaCompra { get; set; }
        public decimal FonaviCompra { get; set; }
        public decimal ServicioCompra { get; set; }
        public decimal IgvExteriorCompra { get; set; }
        public List<OrdenCompraAdmDetalleResponse> Detalles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    /// <summary>
    /// Fila liviana para la grilla de búsqueda (columnas del SELECT de
    /// Logi_sp_Listado_MOCompras_Pedientes / _Aprobacion).
    /// </summary>
    public class OrdenCompraAdmListItemResponse
    {
        public string Code { get; set; } = default!;
        public string Estado { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public DateTime FechaEntrega { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TramiteCode { get; set; } = default!;
        public string Motivo { get; set; } = default!;
        public decimal TotalCompra { get; set; }
        public string? CreatedBy { get; set; }
        public string? AprobadoPor { get; set; }
    }
}
