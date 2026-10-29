using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Stock de un producto en una planta. Catálogo del módulo de Facturación y
    /// Ventas.
    ///
    /// No viene de una tabla legacy del script recibido: se construye directamente
    /// con el esquema profesional ya definido para esta entidad (Id autoincremental;
    /// FK a Comunes.Planta y FK compuesta a Producto). Sin IsActive: un registro de
    /// stock en cero sigue siendo un registro vigente, no uno "inactivo".
    ///
    /// Un solo registro por (Planta, Producto): ver índice único en la configuración.
    /// La cantidad se reemplaza en cada Update; si el negocio necesita historial de
    /// movimientos, ese es un módulo aparte (Kardex/Movimientos de stock), no esta
    /// entidad.
    /// </summary>
    public class StockProducto : IAuditableEntity
    {
        public long Id { get; set; }

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public decimal Cantidad { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
