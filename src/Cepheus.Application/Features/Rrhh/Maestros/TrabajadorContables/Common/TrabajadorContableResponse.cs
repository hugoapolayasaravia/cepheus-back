namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common
{
    public class TrabajadorContableResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public int NumeroItem { get; set; }
        public string? CuentaContable { get; set; }
        public string? Tipo { get; set; }
        public decimal Porcentaje { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
