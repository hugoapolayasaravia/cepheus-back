using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GenerarAsientoCancelacionVenta
{
    /// <summary>
    /// Genera el asiento contable de la cancelación (legacy ue_set_btn_generar + fc_genasicancelacion, asiento tipo 07)
    /// con la plantilla activa del proceso CancelacionVenta. En el mismo paso:
    ///   - guarda el asiento en la cancelación;
    ///   - cheque (02 / 07): le deja el asiento y lo marca Cobrado (el cheque diferido de tipo 02 sigue Pendiente
    ///     hasta liquidarse);
    ///   - letras (03 / 11): les deja el asiento y las marca emitidas (legacy impresion_let = 'S');
    ///   - cheque diferido (07): la fecha de la cancelación (fecha diferida) ya debe haber llegado;
    ///   - recalcula totales de la cancelación (pasa a Cancelado si ya no queda saldo) y el saldo de cada documento.
    /// </summary>
    public record GenerarAsientoCancelacionVentaCommand(string Code, byte[] RowVersion)
        : IRequest<CancelacionVentaResponse>;
}
