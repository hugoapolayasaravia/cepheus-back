using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Unidad de negocio, con jerarquía propia (unidad padre / unidades
    /// hijas). Usado en el módulo de Logística.
    ///
    /// Legacy: dbo.TUnidadNegocio (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Une      -> Code (PK natural, char(6))
    ///   Descripcion_Une -> Name (nullable en el legacy — se respeta esa
    ///                      nulabilidad tal cual, a diferencia del resto de
    ///                      catálogos donde Name es obligatorio)
    ///   Codigo_une_sup  -> ParentCode (FK auto-referenciada -> el propio
    ///                      Code, nullable: null = unidad raíz)
    ///
    /// La FK auto-referenciada del legacy (R_211) no declara ON DELETE
    /// CASCADE — de hecho SQL Server no lo permite en una auto-referencia
    /// por rutas de cascada ambiguas. Se configura como Restrict abajo.
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class UnidadNegocio : IAuditableEntity
    {
        /// <summary>
        /// Código de la unidad de negocio (PK natural, 6 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string? Name { get; set; }

        /// <summary>
        /// Código de la unidad de negocio padre. Null si es una unidad raíz.
        /// </summary>
        public string? ParentCode { get; set; }
        public UnidadNegocio? Parent { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;

        // Navegación
        public ICollection<UnidadNegocio> Children { get; set; } = new List<UnidadNegocio>();
    }
}