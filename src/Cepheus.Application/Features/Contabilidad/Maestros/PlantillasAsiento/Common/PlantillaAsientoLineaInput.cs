using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common
{
    public record PlantillaAsientoLineaInput(
        int Order,
        ConceptoAsiento Concept,
        NaturalezaAsiento Nature,
        OrigenCuentaAsiento AccountSource,
        string? AccountCode,
        OrigenAnexoAsiento AnnexSource,
        OrigenDescripcionAsiento DescriptionSource,
        bool InvertsOnCreditNote,
        bool IsDestination,
        TipoContabilidad AccountingType,
        string? ValeTypeCode = null
    );
}
