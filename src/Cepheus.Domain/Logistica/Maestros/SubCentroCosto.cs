using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// SubCentro de costo, con jerarquía propia (subcentro padre / hijos) y
    /// asociado opcionalmente a un Centro de Costo y obligatoriamente a una
    /// Planta.
    ///
    /// Legacy: dbo.MSubCentroCosto (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Scc      -> Code (PK natural, char(6), correlativo global —
    ///                       mismo criterio que UnidadNegocio: sin
    ///                       codificación jerárquica en el propio código)
    ///   Codigo_Cos      -> CentroCostoCode (FK -> CentroCosto.Code,
    ///                       nullable tal cual el legacy)
    ///   Descripcion_Scc -> Name
    ///   Estado_Scc      -> reemplazado por IsActive estándar
    ///   Codigo_cta,
    ///   codigo_tip      -> AccountingAccountCode,
    ///                       AccountingAttachmentTypeCode: campos inertes,
    ///                       reservados para el módulo de Contabilidad
    ///                       (mismo criterio que en Articulo)
    ///   Codigo_Pla      -> PlantaCode (FK -> Comunes.Planta.Code)
    ///   Codigo_Scc_Sup  -> ParentCode (auto-referencia, nullable; mismo
    ///                       criterio que UnidadNegocio: SÍ editable en
    ///                       Update, porque reubicar en la jerarquía es el
    ///                       caso de uso normal del campo)
    /// </summary>
    public class SubCentroCosto : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string? CentroCostoCode { get; set; }
        public CentroCosto? CentroCosto { get; set; }

        public string Name { get; set; } = default!;

        public string? AccountingAccountCode { get; set; }
        public string? AccountingAttachmentTypeCode { get; set; }

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string? ParentCode { get; set; }
        public SubCentroCosto? Parent { get; set; }
        public ICollection<SubCentroCosto> Children { get; set; } = new List<SubCentroCosto>();

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
