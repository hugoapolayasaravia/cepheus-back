using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.AnularOperacionChequeVenta
{
    /// <summary>
    /// Deshace la última operación del cheque (menú Anular del legacy):
    ///   - Cheque NO diferido rebotado: vuelve a Cobrado y se anula su asiento de rebote.
    ///   - Cheque diferido rebotado: vuelve a Pendiente (o a Cobrado si ya estaba liquidado).
    ///   - Cheque diferido liquidado: se borra la operación y vuelve a Pendiente.
    /// No se puede deshacer si el cheque rebotado ya se aplicó en otra cancelación.
    /// </summary>
    public record AnularOperacionChequeVentaCommand(
        string ClienteCode,
        string BancoCode,
        string Number,
        byte[] RowVersion
    ) : IRequest<ChequeVentaResponse>;
}
