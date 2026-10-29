using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Equipo sobre el que se ejecutan las Órdenes de Trabajo (ej. un motor,
    /// un sistema dentro de una planta). Maestro del módulo Mantenimiento,
    /// con jerarquía propia (Nivel) y relación a Logística.
    ///
    /// Legacy: dbo.MOrigenes (SQL Server) — el nombre "Origenes" es legado de
    /// "origen de la falla"; se renombra a Equipo por ser más representativo.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Ori       -> Code (PK natural, varchar(8), código
    ///                       mnemotécnico manual — ej. "F1MX2601" en el dato
    ///                       de ejemplo, no correlativo numérico)
    ///   Descripcion_Ori  -> Name
    ///   Codigo_Scc       -> SubCentroCostoCode (FK -> Logistica.Maestros.
    ///                       SubCentroCosto.Code, ya existe, nullable tal
    ///                       cual el legacy)
    ///   Nivel            -> Nivel (legacy char(1); se modela como int, más
    ///                       natural para una jerarquía de niveles)
    ///   Estado_Ori       -> IsActive ('A'/otro -> bool, estándar)
    ///
    /// La columna "Codigo_spla" mencionada en el extended property del
    /// legacy no existe realmente en la tabla — se omite (huérfana en el
    /// script original).
    /// </summary>
    public class Equipo : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public int Nivel { get; set; }

        public string? SubCentroCostoCode { get; set; }
        public SubCentroCosto? SubCentroCosto { get; set; }

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
