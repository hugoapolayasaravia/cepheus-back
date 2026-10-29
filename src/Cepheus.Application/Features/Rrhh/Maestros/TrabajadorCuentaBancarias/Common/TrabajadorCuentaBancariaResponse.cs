namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common
{
    public class TrabajadorCuentaBancariaResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? TipoCuentaCode { get; set; }
        public string? BancoCode { get; set; }
        public string? MonedaCode { get; set; }
        public string? NumeroCuenta { get; set; }
        public string TipoOperacion { get; set; } = default!;
        public bool Principal { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
