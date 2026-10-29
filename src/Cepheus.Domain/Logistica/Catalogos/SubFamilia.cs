using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// SubFamilia de artículos, segundo nivel de la clasificación de inventario
    /// (Familia -> SubFamilia). Usado en el módulo de Logística.
    ///
    /// Legacy: dbo.TMSubFamilias (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_SFam       -> Code (PK natural, char(4))
    ///   Codigo_Fam        -> FamiliaCode (FK -> TMFamilias.Codigo_Fam)
    ///   Descripcion_SFam  -> Name
    ///
    /// El legacy define la FK con ON UPDATE CASCADE y sin ON DELETE explícito
    /// (default NO ACTION en SQL Server) — se replica como Restrict abajo en
    /// SubFamiliaConfiguration. En la práctica el Code de Familia se trata como
    /// inmutable desde la aplicación, así que el CASCADE de update no se ejercita.
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class SubFamilia : IAuditableEntity
    {
        /// <summary>
        /// Código de la subfamilia (PK natural, 4 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string FamiliaCode { get; set; } = default!;
        public Familia Familia { get; set; } = default!;

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