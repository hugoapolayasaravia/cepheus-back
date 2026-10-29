namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common
{
    public class ListaPrecioResponse
    {
        public long Id { get; set; }
        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
