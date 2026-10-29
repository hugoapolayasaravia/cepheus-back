using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Línea de una plantilla de asiento. PK: (TemplateCode, Order).
    /// </summary>
    public class PlantillaAsientoLinea : IAuditableEntity
    {
        public string TemplateCode { get; set; } = default!;
        public PlantillaAsiento Template { get; set; } = default!;

        /// <summary>Orden de generación de la línea (1, 2, 3, ...).</summary>
        public int Order { get; set; }

        public ConceptoAsiento Concept { get; set; }

        public NaturalezaAsiento Nature { get; set; }

        public OrigenCuentaAsiento AccountSource { get; set; }

        /// <summary>Cuenta contable. Obligatoria si AccountSource = Fija; debe ser null en los demás casos.</summary>
        public string? AccountCode { get; set; }
        public CuentaContable? Account { get; set; }

        public OrigenAnexoAsiento AnnexSource { get; set; } = OrigenAnexoAsiento.Ninguno;

        public OrigenDescripcionAsiento DescriptionSource { get; set; } = OrigenDescripcionAsiento.GlosaDocumento;

        /// <summary>
        /// Si es verdadero, cuando el comprobante es nota de crédito la línea cambia de naturaleza
        /// (Debe pasa a Haber y viceversa), como hace el legacy con el tipo de documento 07.
        /// </summary>
        public bool InvertsOnCreditNote { get; set; }

        /// <summary>Destino_det del legacy: la línea se marca como línea de destino.</summary>
        public bool IsDestination { get; set; }

        public TipoContabilidad AccountingType { get; set; } = TipoContabilidad.Todos;

        /// <summary>
        /// Solo DetalleVale: tipo de vale al que aplica la línea (por ejemplo "TRA" = transferencia). Las líneas sin tipo
        /// son las generales: se usan para los vales cuyo tipo no tiene una línea propia.
        /// </summary>
        public string? ValeTypeCode { get; set; }
        public TipoVale? ValeType { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
