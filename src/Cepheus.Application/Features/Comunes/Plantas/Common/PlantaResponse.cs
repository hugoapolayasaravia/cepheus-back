namespace Cepheus.Application.Features.Comunes.Plantas.Common
{
    public class PlantaResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? LegalName { get; set; }
        public string Address { get; set; } = default!;
        public string? AddressComplement { get; set; }
        public string? UbigeoCode { get; set; }
        public string? ManagerName { get; set; }
        public bool HasWarehouse { get; set; }
        public bool IsProductionPlant { get; set; }
        public bool IsProject { get; set; }
        public bool RequiresApprovals { get; set; }
        public bool AppliesDetraction { get; set; }
        public string? StatusCode { get; set; }
        public string NumeroGve { get; set; } = default!;
        public string NumeroFve { get; set; } = default!;
        public string NumeroDve { get; set; } = default!;
        public string NumeroBve { get; set; } = default!;
        public string NumeroCve { get; set; } = default!;
        public string NumeroLet { get; set; } = default!;
        public string NumeroRet { get; set; } = default!;
        public string GuiaNum { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
