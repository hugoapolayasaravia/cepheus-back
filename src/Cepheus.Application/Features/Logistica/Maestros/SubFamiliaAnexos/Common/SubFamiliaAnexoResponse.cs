namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;

public class SubFamiliaAnexoResponse
{
    public string SubCentroCostoCode { get; set; } = default!;
    public string SubCentroCostoName { get; set; } = default!;
    public string? CentroCostoCode { get; set; }
    public string SubFamiliaCode { get; set; } = default!;
    public string SubFamiliaName { get; set; } = default!;
    public string AccountingAccountCode { get; set; } = default!;
    public string AccountingAccountName { get; set; } = default!;
    public string? AccountingAttachmentTypeCode { get; set; }
    public string? AccountingAttachmentTypeName { get; set; }
    public string? AccountingAttachmentCode { get; set; }
    public string? AccountingAttachmentName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = default!;
}
