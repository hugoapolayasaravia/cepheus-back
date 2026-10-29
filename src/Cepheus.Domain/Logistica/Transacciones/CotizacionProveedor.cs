// Cepheus.Domain/Logistica/Transacciones/CotizacionProveedor.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Un proveedor participante en una Cotización, con su oferta
    /// consolidada. Legacy: dbo.RProvCotizacion.
    /// Selección por proveedor completo (confirmado, por ahora).
    /// </summary>
    public class CotizacionProveedor : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string CotizacionCode { get; set; } = default!;
        public Cotizacion Cotizacion { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public decimal NetoCotizacion { get; set; }
        public decimal IgvCotizacion { get; set; }
        public decimal TotalCotizacion { get; set; }

        public string Observaciones { get; set; } = string.Empty;

        public EstadoProveedorCotizacion Estado { get; set; } = EstadoProveedorCotizacion.Invitado;

        public DateTime? FechaRespuesta { get; set; }

        public ICollection<CotizacionProveedorDetalle> Detalles { get; set; } = new List<CotizacionProveedorDetalle>();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}