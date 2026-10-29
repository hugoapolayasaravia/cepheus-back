using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Artículo del inventario. Entidad maestra central del módulo de
    /// Logística.
    ///
    /// Legacy: dbo.MArticulos (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Art          -> Code (PK natural, char(7), correlativo)
    ///   Descripcion_Art     -> Name
    ///   Codigo_Uni          -> UnidadMedidaCode (FK -> UnidadMedida.Code)
    ///   Codigo_sfa          -> SubFamiliaCode (FK -> SubFamilia.Code). NOTA:
    ///                          el script legacy tiene un typo — el constraint
    ///                          FK_MArticulos_TMSubFamilias declara la columna
    ///                          como "Codigo_Fam", pero la columna real de la
    ///                          tabla es "Codigo_sfa" (char4, calza con
    ///                          TMSubFamilias.Codigo_SFam). Se usa el nombre
    ///                          real de columna, confirmado con el negocio.
    ///   Stock_Min           -> MinStock
    ///   Stock_Max           -> MaxStock
    ///   Stock_Ent           -> IncomingStock
    ///   L_Time              -> LeadTimeDays. Estimar cuánto tardará en llegar un pedido.
    ///   T_ABC               -> AbcClass (enum A/B/C). Priorizar el control según la importancia del artículo.
    ///   Codigo_Tar          -> TipoArticuloCode (FK -> TipoArticulo.Code)
    ///   Codigo_Pla          -> PlanCode (FK -> PlanArticulo.Code)
    ///   Cod_Fabrica         -> ManufacturerCode (texto libre, sin catálogo)
    ///   Codigo_Est          -> reemplazado por IsActive estándar (decisión:
    ///                          se deja simple por ahora; se evalúa un
    ///                          catálogo de estados de artículo más adelante
    ///                          si el negocio lo requiere)
    ///   Observaciones_Art   -> Observations
    ///   Codigo_Tpr, Codigo_Prd  -> SalesTypeCode, SalesProductCode: campos
    ///                          inertes (sin FK ni validación), reservados
    ///                          para cuando exista el módulo de Ventas
    ///   IND_CONVENIO        -> IsAgreement (bool, 'S'/'N' -> true/false)
    ///   Propiedad01         -> RequiresHorometro (bool, 'S' -> true): el artículo exige registrar
    ///                          el horómetro (Propiedad01 de la línea) al salir en un Vale de Salida
    ///   Codigo_Cta, Codigo_Tip  -> AccountingAccountCode,
    ///                          AccountingAttachmentTypeCode: campos inertes,
    ///                          reservados para el módulo de Contabilidad.
    ///                          Codigo_Tip es NOT NULL en el legacy (default
    ///                          ''); acá se modela nullable porque, sin el
    ///                          módulo de Contabilidad, forzar un valor no
    ///                          tiene sentido de negocio.
    ///   Planta_Ori          -> PlantOriginCode (FK -> Comunes.Planta.Code)
    ///
    /// Columnas NO incluidas: FEC_ACTUALIZACION_CA y FEC_ACTUALIZACION_CA_PT
    /// aparecen en un ALTER TABLE ... ADD CONSTRAINT DEFAULT del script
    /// legacy, pero la columna nunca se declara en el CREATE TABLE provisto
    /// (inconsistencia del propio script). Se omiten hasta confirmar su
    /// existencia real y su propósito.
    /// </summary>
    public class Articulo : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;

        public string UnidadMedidaCode { get; set; } = default!;
        public UnidadMedida UnidadMedida { get; set; } = default!;

        public string SubFamiliaCode { get; set; } = default!;
        public SubFamilia SubFamilia { get; set; } = default!;

        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public decimal IncomingStock { get; set; }
        public decimal LeadTimeDays { get; set; }

        public AbcClass AbcClass { get; set; }

        public string TipoArticuloCode { get; set; } = default!;
        public TipoArticulo TipoArticulo { get; set; } = default!;

        public string PlanCode { get; set; } = default!;
        public PlanArticulo Plan { get; set; } = default!;

        public string? ManufacturerCode { get; set; }

        public string Observations { get; set; } = string.Empty;

        public string? SalesTypeCode { get; set; }
        public string? SalesProductCode { get; set; }

        public bool IsAgreement { get; set; }
        public bool RequiresHorometro { get; set; }

        public string? AccountingAccountCode { get; set; }
        public string? AccountingAttachmentTypeCode { get; set; }

        public string? PlantOriginCode { get; set; }
        public Planta? PlantOrigin { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
