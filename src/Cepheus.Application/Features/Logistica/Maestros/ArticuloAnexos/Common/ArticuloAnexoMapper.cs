using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common
{
    public static class ArticuloAnexoMapper
    {
        public static ArticuloAnexoResponse Map(ArticuloAnexo e) => new()
        {
            SubCentroCostoCode = e.SubCentroCostoCode,
            ArticuloCode = e.ArticuloCode,
            AccountingAccountCode = e.AccountingAccountCode,
            AccountingAttachmentTypeCode = e.AccountingAttachmentTypeCode,
            AccountingAttachmentCode = e.AccountingAttachmentCode,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };

        public static string? Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
