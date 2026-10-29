using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Moneda (ej. Soles, Dólares). Entidad maestra compartida entre módulos
    /// (Ventas, Compras, Logística, etc.) para el manejo de importes multimoneda.
    ///
    /// Legacy: tabla "moneda" (MySQL).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo               -> Code            (ISO 4217 alfabético, ej. "PEN", "USD")
    ///   nombre               -> Name
    ///   simbolo              -> Symbol
    ///   codigo_numerico      -> NumericCode
    ///   decimales            -> DecimalPlaces
    ///   estado               -> IsActive
    ///   fecha_creacion       -> CreatedAt
    ///   fecha_actualizacion   -> UpdatedAt
    /// </summary>
    public class Moneda : IAuditableEntity
    {
        /// <summary>
        /// Código ISO 4217 alfabético. Ejemplo: PEN, USD, EUR.
        /// Es la clave primaria de la entidad.
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;
        public string? Symbol { get; set; }
        public string? NumericCode { get; set; }
        public int DecimalPlaces { get; set; } = 2;
        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}