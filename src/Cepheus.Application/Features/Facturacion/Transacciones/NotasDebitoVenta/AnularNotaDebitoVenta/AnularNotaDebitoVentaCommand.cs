using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.AnularNotaDebitoVenta
{
    /// <summary>
    /// Anula la nota de débito. Como el legacy (ue_set_btn_eli_bor + ad_sp_anula_documento): anula el asiento de
    /// provisión si lo tiene, deja la nota Anulada y elimina sus líneas. La fecha de anulación es hoy, salvo que
    /// la nota sea de un mes anterior: entonces es el último día del mes de emisión.
    /// </summary>
    public record AnularNotaDebitoVentaCommand(string PlantaCode, string Number, string Reason)
        : IRequest<NotaDebitoVentaResponse>;
}
