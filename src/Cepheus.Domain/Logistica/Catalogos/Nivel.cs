using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Nivel jerárquico de aprobación (ej. Nivel 0, Nivel 1...). Catálogo
    /// del submódulo de Gestión de Aprobación de Transacciones, dentro de
    /// Logística.
    ///
    /// Legacy: dbo.TNiveles (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Niv       -> Code (PK natural, char(2)). A diferencia de los
    ///                       catálogos mnemotécnicos de Mantenimiento, acá
    ///                       los códigos del legacy ("00".."05") son
    ///                       puramente correlativos -> se genera con
    ///                       SequentialCodeGenerator, igual que Bancos.
    ///   Descripcion_Niv  -> Name
    ///   Fecha_Ini        -> FechaInicio (default hoy)
    ///   Fecha_Fin        -> FechaFin (nullable — null = vigente sin fecha
    ///                       de cierre, igual que el legacy)
    ///   Ind_Vigencia     -> IsActive (estándar del sistema)
    /// </summary>
    public class Nivel : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

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
