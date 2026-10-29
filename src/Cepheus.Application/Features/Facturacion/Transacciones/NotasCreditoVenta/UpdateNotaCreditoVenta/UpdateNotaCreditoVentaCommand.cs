using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.UpdateNotaCreditoVenta
{
    /// <summary>
    /// Modificación de una nota de crédito Pendiente. Igual que el legacy (ue_set_btn_mod_dw),
    /// solo se pueden cambiar la fecha de emisión y la glosa; las líneas no se editan.
    /// Al cambiar la fecha se recalculan tipo de cambio e IGV.
    /// </summary>
    public record UpdateNotaCreditoVentaCommand(
        string PlantaCode,
        string Number,
        DateTime IssueDate,
        string Description,
        byte[] RowVersion
    ) : IRequest<NotaCreditoVentaResponse>;
}
