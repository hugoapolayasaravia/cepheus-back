using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Flete: tarifa de transporte aplicable a una cotización (independiente
    /// del transporte por producto/vehículo del módulo de despacho). Catálogo
    /// del módulo de Facturación y Ventas.
    ///
    /// Legacy: dbo.MFletes (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_fle       -> Code (PK natural, char(2), correlativo)
    ///   descripcion_fle  -> Name (nvarchar(20))
    ///   monto_fle        -> Amount (numeric(12,2), default 0)
    ///   flag_fle         -> IsDefault (char(1) 'S'/'N', sin comentario en el
    ///                       legacy sobre su propósito exacto). PENDIENTE
    ///                       CONFIRMAR: se asume que indica si este flete se
    ///                       aplica automáticamente al crear una cotización
    ///                       (análogo a un flete "por defecto"); confirmar
    ///                       significado real antes de migrar datos.
    ///   estado           -> IsActive (char(2) FK TEstados, default '05'
    ///                       Activo) -> reemplazado por bool, mismo criterio
    ///                       que FormaPagoVenta/Vendedor/Cobrador.
    /// </summary>
    public class Flete : IAuditableEntity
    {
        /// <summary>Código del flete (PK natural, 2 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public decimal Amount { get; set; }

        /// <summary>Ver nota PENDIENTE CONFIRMAR en el comentario de la clase.</summary>
        public bool IsDefault { get; set; }

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
