using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Proveedor (por RUC) no afecto a la retención del IGV. Maestro del módulo de Contabilidad:
    /// la cancelación a proveedores lo consulta para decidir si retiene o no.
    ///
    /// Legacy: dbo.noafectos (base auditoria, SQL Server); la consulta la hacía fg_afecto.
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   ruc         -> Ruc (char(11), parte de la PK)
    ///   Tipo        -> Type (char(1), parte de la PK; el legacy no lo usa para decidir)
    ///   Nombre      -> Name (nvarchar(70))
    ///   resolucion  -> Resolution (nvarchar(50))
    ///
    /// Regla (fg_afecto): el proveedor es afecto salvo que su RUC exista en esta tabla, sin
    /// importar el Tipo. IsActive no existe en la tabla legacy; se agrega por consistencia y
    /// solo los registros activos dejan de afectar al proveedor.
    /// </summary>
    public class NoAfectoRetencion : IAuditableEntity
    {
        public string Ruc { get; set; } = default!;

        public string Type { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Resolution { get; set; } = default!;

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
