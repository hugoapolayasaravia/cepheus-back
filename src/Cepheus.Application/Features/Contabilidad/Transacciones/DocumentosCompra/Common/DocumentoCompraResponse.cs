namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common
{
    public class DocumentoCompraDetalleResponse
    {
        public int ItemNumber { get; set; }
        public string PlantaCode { get; set; } = default!;
        public string CuentaContableCode { get; set; } = default!;
        public string? AnexoTipoCode { get; set; }
        public string? AnexoCode { get; set; }
        public string Detalle { get; set; } = default!;
        public decimal Cantidad { get; set; }
        public string UnidadMedidaCode { get; set; } = default!;
        public decimal PrecioSol { get; set; }
        public decimal ParcialSol { get; set; }
        public string? NumeroGuiaVenta { get; set; }
        public string? ProductoTipoCode { get; set; }
        public string? ProductoCode { get; set; }
    }

    public class DocumentoCompraResponse
    {
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string FormaPagoCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public DateOnly? TipoCambioFecha { get; set; }
        /// <summary>Tipo de cambio venta de TipoCambioFecha (1 si es soles). Se consulta en Comunes.TipoCambio.</summary>
        public decimal? TipoCambioVenta { get; set; }
        public decimal ParcialSol { get; set; }
        public decimal IgvSol { get; set; }
        public decimal NoGravableSol { get; set; }
        public decimal RentaSol { get; set; }
        public decimal FonaviSol { get; set; }
        public decimal ServicioSol { get; set; }
        public decimal IgvExteriorSol { get; set; }
        public decimal TotalSol { get; set; }
        public string Glosa { get; set; } = default!;
        public string Origen { get; set; } = default!;
        public string? Procedencia { get; set; }
        public string Estado { get; set; } = default!;
        public string? TipoBienCode { get; set; }
        public string? TipoOperacionCode { get; set; }
        public string? OperacionDetraccion { get; set; }
        public string? ConstanciaDetraccion { get; set; }
        public DateTime? FechaConstanciaDetraccion { get; set; }
        public string? NumeroDetraccion { get; set; }
        public DateTime? FechaEmisionDetraccion { get; set; }
        public decimal MontoDetraccionSol { get; set; }
        public string? DocumentoOrigenComprobantePagoCode { get; set; }
        public string? DocumentoOrigenNumero { get; set; }
        public string? OrdenCompraAdministracionCode { get; set; }
        public decimal PagadoSol { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public decimal PendienteSol { get; set; }
        public string? AsientoProvisionCode { get; set; }
        public List<DocumentoCompraDetalleResponse> Detalles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

    /// <summary>Fila de la grilla de búsqueda: columnas del SELECT de co_sp_Listado_Doc_Compras.</summary>
    public class DocumentoCompraListItemResponse
    {
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public DateTime FechaEmision { get; set; }
        /// <summary>FecProceso_dco del legacy (CreatedAt).</summary>
        public DateTime FechaProceso { get; set; }
        public string FormaPagoCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public DateOnly? TipoCambioFecha { get; set; }
        /// <summary>Venta_dco del legacy: se consulta en Comunes.TipoCambio (soles = 1).</summary>
        public decimal? TipoCambioVenta { get; set; }
        public decimal TotalSol { get; set; }
        public string Estado { get; set; } = default!;
        public string? AsientoProvisionCode { get; set; }
    }
}
