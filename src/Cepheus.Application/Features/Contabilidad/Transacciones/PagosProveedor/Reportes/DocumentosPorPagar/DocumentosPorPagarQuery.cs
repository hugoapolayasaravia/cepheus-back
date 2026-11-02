using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.DocumentosPorPagar
{
    /// <summary>
    /// Legacy ad_rpt_com_docxpagar: documentos pendientes de pago a una fecha de corte, con su antigüedad
    /// (días desde la recepción): 0-7, 8-15, 16-30, 31-45 y más de 45.
    /// PlantaCode vacío o "ZZ" = todas; ProveedorCode vacío = todos; sin fecha de corte = hoy.
    /// </summary>
    public class DocumentosPorPagarQuery : IRequest<List<DocumentoPorPagarRowResponse>>
    {
        public string? PlantaCode { get; set; }
        public string? ProveedorCode { get; set; }
        public DateTime? FechaCorte { get; set; }
    }

    public class DocumentoPorPagarRowResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public string ProveedorRuc { get; set; } = default!;
        public string ProveedorName { get; set; } = default!;
        public string FormaPagoCode { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaCorte { get; set; }
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public decimal TotalSol { get; set; }
        public decimal PagadoSol { get; set; }
        public decimal PendienteSol { get; set; }
        public int Dias { get; set; }
        public decimal Periodo1Sol { get; set; }
        public decimal Periodo2Sol { get; set; }
        public decimal Periodo3Sol { get; set; }
        public decimal Periodo4Sol { get; set; }
        public decimal Periodo5Sol { get; set; }
    }
}
