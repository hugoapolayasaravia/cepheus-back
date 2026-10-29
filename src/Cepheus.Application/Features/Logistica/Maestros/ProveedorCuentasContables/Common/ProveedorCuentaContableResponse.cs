namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common
{
    public class ProveedorCuentaContableResponse
    {
        public string ProveedorCode { get; set; } = default!;
        public string? ProveedorName { get; set; }
        public string MonedaCode { get; set; } = default!;
        public string AccountingAccountCode { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
