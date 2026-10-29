using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Tipo de transacción/documento sujeto a aprobación (ej. "OC" = Orden
    /// de Compra, "SC" = Orden de Requisición). Catálogo del submódulo de
    /// Gestión de Aprobación de Transacciones, dentro de Logística.
    ///
    /// Legacy: dbo.MTransaccion (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Tras       -> Code (PK natural, char(2), código
    ///                        mnemotécnico manual — igual criterio que
    ///                        UnidadMedida/los catálogos de Mantenimiento)
    ///   Descripcion_Tras  -> Name
    ///
    /// La tabla legacy dbo.TTransacciones (combinaciones
    /// Codigo_Tras + Codigo_Une habilitadas) se elimina por redundante —
    /// ver análisis previo aprobado: esa información se desprende de que
    /// existan filas en RangoAprobacion para esa combinación.
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class TipoTransaccion : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

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
