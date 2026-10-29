using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common
{
    public class ProveedorCuentaResponse
    {
        public int Id { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string BancoCode { get; set; } = default!;
        public AccountType AccountType { get; set; }
        public string AccountNumber { get; set; } = default!;
        public string? InterbankCode { get; set; }
        public string MonedaCode { get; set; } = default!;
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
