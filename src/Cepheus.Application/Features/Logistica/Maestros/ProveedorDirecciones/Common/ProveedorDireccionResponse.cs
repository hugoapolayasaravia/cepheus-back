using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common
{
    public class ProveedorDireccionResponse
    {
        public int Id { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public AddressType AddressType { get; set; }
        public string Address { get; set; } = default!;
        public string? UbigeoCode { get; set; }
        public string? Reference { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
