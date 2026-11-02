using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CreateCancelacionVenta
{
    /// <summary>
    /// Registra una cancelación (legacy w_fac_reg_cancelaciones, ue_set_btn_agr_var + ue_set_btn_grb_data): el cliente
    /// paga y el pago se aplica a documentos. Según el tipo de pago, además nace un cheque (02, 07) o una letra (03).
    /// Los documentos se pueden enviar aquí o agregarse después. El tipo 11 (Letras Varias) se crea con su propio proceso.
    /// </summary>
    public class CreateCancelacionVentaCommand : IRequest<CancelacionVentaResponse>
    {
        public string PlantaCode { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;

        /// <summary>Fecha de la cancelación. Para el cheque diferido (07) se toma la fecha diferida.</summary>
        public DateTime Date { get; set; }
        public string CobradorCode { get; set; } = default!;

        /// <summary>ISO (PEN, USD).</summary>
        public string MonedaCode { get; set; } = default!;

        /// <summary>01 efectivo, 02 cheque, 03 letras, 04 canje, 05..., 07 cheque diferido, 08..., etc. (catálogo de tipos de pago).</summary>
        public string TipoPagoCode { get; set; } = default!;

        /// <summary>Número del cheque (02, 07) o del voucher; en letras (03) lo asigna el sistema.</summary>
        public string? PaymentNumber { get; set; }

        /// <summary>Cuenta bancaria de depósito.</summary>
        public string? AccountNumber { get; set; }

        /// <summary>Banco del cheque (obligatorio en 02 y 07).</summary>
        public string? BancoDocCode { get; set; }

        /// <summary>Banco de depósito (obligatorio salvo canje 04).</summary>
        public string? BancoDepCode { get; set; }

        /// <summary>Cheque diferido (solo 02; el 07 siempre lo es).</summary>
        public bool IsDeferred { get; set; }

        /// <summary>Fecha diferida / presentación (07, 02 diferido) o vencimiento de la letra (03).</summary>
        public DateTime? DeferredDate { get; set; }
        public string? OperationNumber { get; set; }
        public DateTime? OperationDate { get; set; }
        public string? Description { get; set; }

        /// <summary>Importe de la cancelación en su moneda. En el canje (04) se ignora (siempre 0).</summary>
        public decimal TotalAmount { get; set; }

        public List<CancelacionVentaDocumentoInput> Documents { get; set; } = new();
    }
}
