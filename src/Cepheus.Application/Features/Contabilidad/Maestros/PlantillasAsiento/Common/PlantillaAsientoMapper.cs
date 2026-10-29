using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common
{
    public static class PlantillaAsientoMapper
    {
        public static PlantillaAsientoResponse Map(PlantillaAsiento e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            Process = e.Process,
            TipoAsientoCode = e.TipoAsientoCode,
            PurchaseAccountCode = e.PurchaseAccountCode,
            ComprobantePagoCode = e.ComprobantePagoCode,
            DescriptionText = e.DescriptionText,
            IsActive = e.IsActive,
            Lines = e.Lines
                .OrderBy(l => l.Order)
                .Select(l => new PlantillaAsientoLineaResponse
                {
                    Order = l.Order,
                    Concept = l.Concept,
                    Nature = l.Nature,
                    AccountSource = l.AccountSource,
                    AccountCode = l.AccountCode,
                    AnnexSource = l.AnnexSource,
                    DescriptionSource = l.DescriptionSource,
                    InvertsOnCreditNote = l.InvertsOnCreditNote,
                    IsDestination = l.IsDestination,
                    AccountingType = l.AccountingType,
                    ValeTypeCode = l.ValeTypeCode,
                    CurrencyCode = l.CurrencyCode
                }).ToList(),
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
