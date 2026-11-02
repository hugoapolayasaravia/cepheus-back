using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.UpdateCancelacionVenta
{
    /// <summary>
    /// Modifica la cancelación mientras está Pendiente y sin asiento (legacy ue_set_btn_mod_dw). Siempre se pueden
    /// cambiar cobrador, banco y cuenta de depósito, glosa, operación y fecha diferida / vencimiento. El importe y la fecha
    /// solo se cambian mientras no tenga documentos. Tipo de pago, moneda, cliente, banco y número del cheque no cambian.
    /// </summary>
    public class UpdateCancelacionVentaCommand : IRequest<CancelacionVentaResponse>
    {
        public string Code { get; set; } = default!;
        public string CobradorCode { get; set; } = default!;
        public string? BancoDepCode { get; set; }
        public string? AccountNumber { get; set; }
        public string? Description { get; set; }
        public string? OperationNumber { get; set; }
        public DateTime? OperationDate { get; set; }
        public DateTime? DeferredDate { get; set; }

        /// <summary>Solo sin documentos.</summary>
        public DateTime? Date { get; set; }

        /// <summary>Solo sin documentos.</summary>
        public decimal? TotalAmount { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
