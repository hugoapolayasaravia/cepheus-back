namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common
{
    public class ArticuloAnexoResponse
    {
        public string SubCentroCostoCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public string AccountingAccountCode { get; set; } = default!;
        public string? AccountingAttachmentTypeCode { get; set; }
        public string? AccountingAttachmentCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
