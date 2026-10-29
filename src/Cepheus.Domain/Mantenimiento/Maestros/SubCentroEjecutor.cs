using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Subdivisión de un Centro Ejecutor (cuadrilla específica dentro del
    /// área de mantenimiento). Maestro del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TSubCentroEjecutor (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_sce       -> Code (PK natural, char(4), código
    ///                       mnemotécnico manual)
    ///   Codigo_ce        -> CentroEjecutorCode (FK -> CentroEjecutor.Code,
    ///                       NOT NULL en el legacy)
    ///   Descripcion_Sce  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class SubCentroEjecutor : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string CentroEjecutorCode { get; set; } = default!;
        public CentroEjecutor CentroEjecutor { get; set; } = default!;

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
