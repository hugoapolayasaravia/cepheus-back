// Cepheus.Domain/Logistica/Transacciones/Cotizacion.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Cotización (RFQ): solicitud de precios a uno o más proveedores sobre
    /// una lista de artículos. Legacy: dbo.RCotizaciones.
    ///
    /// Puede nacer de una o más líneas de Pedido (ver CotizacionPedidoOrigen,
    /// vínculo a nivel de línea, no de cabecera), o ser standalone.
    /// </summary>
    public class Cotizacion : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Correlativo por planta (Codigo_Cot, char(6)), sin prefijo fijo.</summary>
        public string Code { get; set; } = default!;

        public DateTime FechaLimite { get; set; }

        public EstadoCotizacion Estado { get; set; } = EstadoCotizacion.Pendiente;

        public string Observaciones { get; set; } = string.Empty;

        /// <summary>Cotización de origen si esta nace de "copiar cotización". Null si es nueva.</summary>
        public string? OriginalCode { get; set; }
        public Cotizacion? Original { get; set; }

        public DateTime? FechaCierre { get; set; }

        public ICollection<CotizacionDetalle> Detalles { get; set; } = new List<CotizacionDetalle>();
        public ICollection<CotizacionProveedor> Proveedores { get; set; } = new List<CotizacionProveedor>();

        // Auditoría (IAuditableEntity) — Usuario_cot/FechaProceso_cot del legacy quedan cubiertos aquí
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}