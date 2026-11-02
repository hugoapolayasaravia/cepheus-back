using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.UpdateLetraVenta
{
    /// <summary>
    /// Corrige una letra por cobrar (legacy menú Modificar): fecha de emisión, vencimiento, aceptación del cliente
    /// y glosa. Si se marca la aceptación (una vez aceptada ya no se puede quitar) y <c>ExchangeDocuments</c> es true,
    /// se canjean los documentos de la cancelación que originó la letra (quedan cancelados).
    /// </summary>
    public record UpdateLetraVentaCommand(
        string PlantaCode,
        string Number,
        DateTime IssueDate,
        DateTime DueDate,
        bool AcceptedByClient,
        string? Description,
        bool ExchangeDocuments,
        byte[] RowVersion
    ) : IRequest<LetraVentaResponse>;
}
