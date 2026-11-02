using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.DocumentosPendientes.GetDocumentosPendientesProveedor
{
    public record GetDocumentosPendientesProveedorQuery(string ProveedorCode)
        : IRequest<List<DocumentoPendientePagoResponse>>;

    public class DocumentoPendientePagoResponse
    {
        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }

        /// <summary>Total del documento (negativo en notas de crédito, como el legacy).</summary>
        public decimal TotalSol { get; set; }

        /// <summary>Pendiente del documento (negativo en notas de crédito).</summary>
        public decimal PendienteSol { get; set; }

        /// <summary>Pago que propone el proceso (legacy ue_control): el pendiente, menos la detracción si no hubo pagos previos.</summary>
        public decimal PagoSugeridoSol { get; set; }

        public string? AsientoProvisionCode { get; set; }
        public bool Detraccion { get; set; }
        public decimal MontoDetraccionSol { get; set; }
    }
}
