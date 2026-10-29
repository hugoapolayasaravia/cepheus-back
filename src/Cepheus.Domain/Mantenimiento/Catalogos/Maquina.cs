using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Catalogos
{
    /// <summary>
    /// Máquina utilizada en la ejecución de una Orden de Trabajo (ej. una
    /// grúa, un montacargas). Catálogo del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TMaquinas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Maq       -> Code (PK natural, char(4))
    ///   Descripcion_Maq  -> Name
    ///
    /// A diferencia de Inspeccion/Especialidad/Oportunidad/Prioridad/
    /// TipoOrden, el script no trae ningún dato de ejemplo para este
    /// catálogo, así que no hay evidencia de que use códigos mnemotécnicos
    /// manuales. Se sigue el mismo criterio de código manual por
    /// consistencia con el resto del módulo — si prefieres que sea
    /// correlativo automático (como Bancos), dímelo y lo cambio.
    /// </summary>
    public class Maquina : IAuditableEntity
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
