namespace Cepheus.Application.Features.Comunes.Ubigeos.Common
{
    public class UbigeoResponse
    {
        public string Code { get; set; } = default!;
        public string Department { get; set; } = default!;
        public string Province { get; set; } = default!;
        public string District { get; set; } = default!;
        public string FullAddress { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
