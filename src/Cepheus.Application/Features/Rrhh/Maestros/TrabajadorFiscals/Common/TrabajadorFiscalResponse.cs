namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common
{
    public class TrabajadorFiscalResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public bool ConInmTrabajador { get; set; }
        public bool Domiciliado { get; set; }
        public bool OtrosIngresosQuinta { get; set; }
        public bool RentaQuintaExonerada { get; set; }
        public bool MadreResFamiliar { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
