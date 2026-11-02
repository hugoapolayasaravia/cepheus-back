using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.AnularPagoProveedor
{
    /// <summary>
    /// Anulación del pago en tres niveles (legacy ue_set_btn_eli_bor):
    ///   AnularOperacion: anula el pago, quita sus documentos (devuelve el saldo a cada documento) y, por
    ///                    obligación, anula el asiento y libera el cheque.
    ///   AnularAsiento:   solo anula el asiento de cancelación; el pago vuelve a Pendiente.
    ///   LiberarCheque:   deja el número de cheque en blanco (solo si no hay asiento o se anula el asiento).
    /// </summary>
    public record AnularPagoProveedorCommand(
        string Code,
        bool AnularOperacion,
        bool AnularAsiento,
        bool LiberarCheque
    ) : IRequest<PagoProveedorResponse>;
}
