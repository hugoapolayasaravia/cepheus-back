namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.Common
{
    public class ProductoResponse
    {
        public string TipoProductoCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string FullCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ShortName { get; set; }
        public string UnitCode { get; set; } = default!;
        public string? AccountingAccountCode { get; set; }
        public string? TransportAccountCode { get; set; }
        public string? CreditNoteAccountCode { get; set; }
        public decimal LengthLimit { get; set; }
        public string? TransportTipoProductoCode { get; set; }
        public string? TransportCode { get; set; }
        public string? CategoryCode { get; set; }
        public string? StrengthCode { get; set; }
        public string? CementTypeCode { get; set; }
        public string? StoneSizeCode { get; set; }
        public string? SlumpCode { get; set; }
        public string? WaterCementRatioCode { get; set; }
        public string? AgeCode { get; set; }
        public string? SpecialConditionCode { get; set; }
        public string? MixProportionCode { get; set; }
        public bool IsPumpable { get; set; }
        public bool IsSubjectToDetraction { get; set; }
        public string? GoodsTypeCode { get; set; }
        public string? OperationTypeCode { get; set; }
        public decimal? CementValue { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
