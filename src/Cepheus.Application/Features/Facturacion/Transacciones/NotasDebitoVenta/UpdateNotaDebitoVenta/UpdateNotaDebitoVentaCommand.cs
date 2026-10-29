using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.UpdateNotaDebitoVenta
{
    /// <summary>
    /// Modificación de una nota de débito Pendiente (no impresa). Como en el legacy se puede cambiar la fecha
    /// de emisión (recalcula tipo de cambio e IGV), la glosa y las líneas (se reemplazan por las enviadas).
    /// </summary>
    public record UpdateNotaDebitoVentaCommand(
        string PlantaCode,
        string Number,
        DateTime IssueDate,
        string Description,
        List<NotaDebitoVentaDetalleInput> Details,
        byte[] RowVersion
    ) : IRequest<NotaDebitoVentaResponse>;
}
