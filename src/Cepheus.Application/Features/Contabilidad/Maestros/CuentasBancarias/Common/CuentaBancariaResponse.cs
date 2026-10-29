namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common
{
    public class CuentaBancariaResponse
    {
        public string BankCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string CurrencyCode { get; set; } = default!;
        public string CheckStart { get; set; } = default!;
        public string CheckEnd { get; set; } = default!;
        public string CheckCurrent { get; set; } = default!;
        public string ControlFlag { get; set; } = default!;
        public string AccountCode { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
