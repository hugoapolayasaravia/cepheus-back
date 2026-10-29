using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Precio de un producto vigente en un rango de fechas. Catálogo del módulo de
    /// Facturación y Ventas.
    ///
    /// No viene de una tabla legacy del script recibido: se construye directamente
    /// con el esquema profesional ya definido para esta entidad (Id autoincremental,
    /// no código; FK compuesta a Producto).
    /// </summary>
    public class ListaPrecio : IAuditableEntity
    {
        public long Id { get; set; }

        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public decimal Precio { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
