namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common
{
    public class CuentaContableResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string TipoCuentaCode { get; set; } = default!;
        public string? TipoAnexoCode { get; set; } = default!;
        public string LinkAccount1Code { get; set; } = default!;
        public string LinkAccount2Code { get; set; } = default!;
        public string CurrencyCode { get; set; } = default!;
        public string? AcmFlag { get; set; } = default!;
        public string? PilotFlag { get; set; } = default!;
        public string? GainsLossesFlag { get; set; } = default!;
        public string ClosingAccountCode { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
