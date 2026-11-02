using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AnularAsientoCancelacionVenta
{
    /// <summary>
    /// Anula el asiento de la cancelación (legacy ue_set_btn_eli_bor con asiento generado + fc_anulaasiento):
    ///   - El período contable de la fecha de la cancelación debe estar abierto.
    ///   - Cheque (02 / 07): debe seguir Cobrado (el 02 diferido, Pendiente); si fue rebotado no se anula.
    ///     Letras (03): no deben tener cancelaciones aplicadas.
    ///   - Anula el asiento; la cancelación vuelve a Pendiente sin asiento. El cheque pierde su asiento (el 07
    ///     vuelve a Pendiente) y la letra deja de estar emitida.
    ///   - Letras Varias (11): además se retiran los documentos (quedan libres para otra operación) y la
    ///     cancelación y sus letras quedan Anuladas.
    /// </summary>
    public record AnularAsientoCancelacionVentaCommand(string Code, string Reason, byte[] RowVersion)
        : IRequest<CancelacionVentaResponse>;
}
