using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CuadrarCancelacionVenta
{
    /// <summary>
    /// Cuadre por diferencia (legacy ue_det_cuadre): lo que sobra o falta entre el importe de la cancelación y lo aplicado
    /// a sus documentos se registra como diferencia en el documento indicado, dejando la cancelación sin saldo. En el
    /// canje (04) deja los totales en cero.
    /// </summary>
    public record CuadrarCancelacionVentaCommand(string Code, string Item, byte[] RowVersion)
        : IRequest<CancelacionVentaResponse>;
}
