using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.RebotarChequeVenta
{
    /// <summary>
    /// Rebota el cheque (menú R del legacy). Cheque NO diferido: genera el asiento de rebote (tipo 01) y el
    /// cheque queda Rebotado (de nuevo una deuda del cliente). Cheque diferido: solo cambia a Rebotado.
    /// </summary>
    public record RebotarChequeVentaCommand(
        string ClienteCode,
        string BancoCode,
        string Number,
        string MotivoReboteCode,
        DateTime ReboteDate,
        byte[] RowVersion
    ) : IRequest<ChequeVentaResponse>;
}
