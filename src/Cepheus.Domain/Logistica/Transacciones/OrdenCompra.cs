// Cepheus.Domain/Logistica/Transacciones/OrdenCompra.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Orden de Compra: documento formal de compra a un Proveedor, cierre
    /// del flujo Pedido -> Cotización -> Orden de Compra. Legacy: dbo.MComprasRes.
    ///
    /// Cambios frente al legacy:
    ///   - Codigo_Est (char(2)) -> Estado (enum EstadoOrdenCompra)
    ///   - Usuario/Fecha_Pro -> cubiertos por CreatedBy/CreatedAt
    ///   - Usuario_Apro/Fecha_Apr -> AprobadoPor/FechaAprobacion
    ///   - Moneda (char(1) legacy) -> MonedaCode FK Comunes.Moneda (3 chars
    ///     ISO), mismo criterio que CotizacionProveedor.MonedaCode
    ///   - Codigo_tdo -> ComprobantePagoId FK Comunes.ComprobantePago
    ///     (corrección: no es TipoDocumento — ver ComprobantePago)
    ///   - Codigo_Trm cubre lo que en pantalla se ve como "Prioridad"
    ///     (confirmado, no es un campo nuevo)
    ///   - Neto_Com/Igv_Com/Monto_Com legacy tenían los nombres cambiados
    ///     (confirmado): Total_Com (legacy) -> NetoCompra, Monto_Com
    ///     (legacy) -> TotalCompra
    ///   - NetoCompra/IgvCompra/RentaCompra/FonaviCompra se calculan
    ///     siempre desde el detalle + ComprobantePago + ControlVentas (ver
    ///     OrdenCompraTotalsCalculator). NoGravableCompra/ServicioCompra/
    ///     IgvExteriorCompra son 100% digitados por el usuario — el
    ///     PowerBuilder original nunca los deriva de una fórmula.
    ///
    /// Trazabilidad a Pedido a nivel de línea: ver OrdenCompraPedidoOrigen
    /// (confirmado, mismo patrón que CotizacionPedidoOrigen).
    /// </summary>
    public class OrdenCompra : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Correlativo por planta: '5' + 5 dígitos (Codigo_Com, char(6)).</summary>
        public string Code { get; set; } = default!;

        public string TipoCompraCode { get; set; } = default!;
        public TipoCompra TipoCompra { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public ComprobantePago? ComprobantePago { get; set; }

        public DateTime FechaEntrega { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string CompradorCode { get; set; } = default!;
        public Comprador Comprador { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public string LugarEnvioCode { get; set; } = default!;
        public LugarEnvio LugarEnvio { get; set; } = default!;

        public string FormaPagoCode { get; set; } = default!;
        public FormaPago FormaPago { get; set; } = default!;

        /// <summary>Cubre lo que la pantalla muestra como "Prioridad" (confirmado).</summary>
        public string TramiteCode { get; set; } = default!;
        public Tramite Tramite { get; set; } = default!;

        public string? Observaciones1 { get; set; }
        public string? Observaciones2 { get; set; }

        public string? NotaCompraCode { get; set; }
        public NotaCompra? NotaCompra { get; set; }

        public string UnidadNegocioCode { get; set; } = default!;
        public UnidadNegocio UnidadNegocio { get; set; } = default!;

        public bool EnviarCorreoProveedor { get; set; }

        public EstadoOrdenCompra Estado { get; set; } = EstadoOrdenCompra.Pendiente;

        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        public string? MotivoRetraso { get; set; }

        public decimal NetoCompra { get; set; }
        public decimal IgvCompra { get; set; }
        public decimal TotalCompra { get; set; }

        /// <summary>Digitado por el usuario, no se calcula.</summary>
        public decimal NoGravableCompra { get; set; }
        /// <summary>Calculado (ComprobantePago.AffectsIncomeTax + ControlVentas.WithholdingPercentage/WithholdingCap).</summary>
        public decimal RentaCompra { get; set; }
        /// <summary>Calculado (ComprobantePago.AffectsFonavi + ControlVentas.FonaviPercentage).</summary>
        public decimal FonaviCompra { get; set; }
        /// <summary>Digitado por el usuario, no se calcula.</summary>
        public decimal ServicioCompra { get; set; }
        /// <summary>Digitado por el usuario, no se calcula.</summary>
        public decimal IgvExteriorCompra { get; set; }

        public ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}