using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.Common
{
    public class ArticuloResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string UnidadMedidaCode { get; set; } = default!;
        public string SubFamiliaCode { get; set; } = default!;
        public string? SubFamiliaName { get; set; }
        public string? UnidadMedidaName { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public decimal IncomingStock { get; set; }
        public decimal LeadTimeDays { get; set; }
        public AbcClass AbcClass { get; set; }
        public string TipoArticuloCode { get; set; } = default!;
        public string PlanCode { get; set; } = default!;
        public string? ManufacturerCode { get; set; }
        public string Observations { get; set; } = default!;
        public string? SalesTypeCode { get; set; }
        public string? SalesProductCode { get; set; }
        public bool IsAgreement { get; set; }
        public string? AccountingAccountCode { get; set; }
        public string? AccountingAttachmentTypeCode { get; set; }
        public string? PlantOriginCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
