using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;

internal static class SubFamiliaAnexoProjection
{
    public static IQueryable<SubFamiliaAnexo> ByKey(
        this IQueryable<SubFamiliaAnexo> query, string subCentroCostoCode, string subFamiliaCode)
        => query.Where(x => x.SubCentroCostoCode == subCentroCostoCode && x.SubFamiliaCode == subFamiliaCode);

    public static IQueryable<SubFamiliaAnexoResponse> ToResponse(this IQueryable<SubFamiliaAnexo> query)
        => query.Select(x => new SubFamiliaAnexoResponse
        {
            SubCentroCostoCode = x.SubCentroCostoCode,
            SubCentroCostoName = x.SubCentroCosto.Name,
            CentroCostoCode = x.SubCentroCosto.CentroCostoCode,
            SubFamiliaCode = x.SubFamiliaCode,
            SubFamiliaName = x.SubFamilia.Name,
            AccountingAccountCode = x.AccountingAccountCode,
            AccountingAccountName = x.CuentaContable.Name,
            AccountingAttachmentTypeCode = x.AccountingAttachmentTypeCode,
            AccountingAttachmentTypeName = x.TipoAnexo != null ? x.TipoAnexo.Name : null,
            AccountingAttachmentCode = x.AccountingAttachmentCode,
            AccountingAttachmentName = x.Anexo != null ? x.Anexo.Name : null,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            RowVersion = x.RowVersion
        });

    public static string? Clean(string? value, bool upper = true)
        => string.IsNullOrWhiteSpace(value) ? null : (upper ? value.Trim().ToUpperInvariant() : value.Trim());
}
