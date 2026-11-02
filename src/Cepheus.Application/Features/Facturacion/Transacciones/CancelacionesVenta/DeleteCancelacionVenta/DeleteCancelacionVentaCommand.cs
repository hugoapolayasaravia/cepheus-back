using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.DeleteCancelacionVenta
{
    /// <summary>
    /// Elimina una cancelación Pendiente, sin asiento y sin documentos (legacy ue_set_btn_eli_bor), junto con el cheque o
    /// la letra que nació con ella.
    /// </summary>
    public record DeleteCancelacionVentaCommand(string Code) : IRequest<Unit>;
}
