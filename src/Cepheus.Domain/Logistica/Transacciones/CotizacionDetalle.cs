// Cepheus.Domain/Logistica/Transacciones/CotizacionDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Línea de artículo a cotizar. Legacy: dbo.DCotizaciones. A diferencia
    /// de PedidoDetalle, aquí ArticuloCode SÍ tiene FK real (el legacy la
    /// declara: FK_DCotizaciones_MArticulos) — no admite líneas de
    /// descripción libre.
    /// </summary>
    public class CotizacionDetalle : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string CotizacionCode { get; set; } = default!;
        public Cotizacion Cotizacion { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        /// <summary>Correlativo de línea solo para orden de despliegue (no es parte de la PK).</summary>
        public int ItemNumber { get; set; }

        public decimal CantidadArticulo { get; set; }

        /// <summary>Líneas de Pedido que aportaron cantidad a este artículo cotizado (control manual, ver clase entidad).</summary>
        public ICollection<CotizacionPedidoOrigen> Origenes { get; set; } = new List<CotizacionPedidoOrigen>();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}