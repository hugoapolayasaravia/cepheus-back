namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common
{
    public class SubCentroCostoResponse
    {
        public string Code { get; set; } = default!;
        public string? CentroCostoCode { get; set; }
        public string Name { get; set; } = default!;
        public string? AccountingAccountCode { get; set; }
        public string? AccountingAttachmentTypeCode { get; set; }
        public string PlantaCode { get; set; } = default!;
        public string? ParentCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
