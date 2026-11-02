using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ComprobanteRetencionPagoProveedor
{
    /// <summary>Legacy ad_rpt_com_retenciones: datos para imprimir el comprobante de retención del pago.</summary>
    public record ComprobanteRetencionPagoProveedorQuery(string Code) : IRequest<ComprobanteRetencionPagoProveedorResponse>;

    public class ComprobanteRetencionPagoProveedorResponse
    {
        public string PagoCode { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public string ProveedorName { get; set; } = default!;
        public string ProveedorRuc { get; set; } = default!;
        public string? NumeroComprobanteRetencion { get; set; }
        public DateTime? FechaComprobanteRetencion { get; set; }
        public List<ComprobanteRetencionItemResponse> Documentos { get; set; } = new();
    }

    public class ComprobanteRetencionItemResponse
    {
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string Serie { get; set; } = default!;
        public string Correlativo { get; set; } = default!;
        public DateTime FechaEmisionDocumento { get; set; }

        /// <summary>Monto afecto a retención (legacy: monretencion_che, impreso como "pago").</summary>
        public decimal BaseRetencionSol { get; set; }

        /// <summary>Retención calculada (legacy: aferetencion_che, impreso como "retenido").</summary>
        public decimal MontoRetenidoSol { get; set; }
    }
}
