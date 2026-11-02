using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.EliminarDocumentoLetraVenta
{
    /// <summary>
    /// Quita un documento de la cancelación que originó la letra (legacy ue_eli_lin_det). Solo si el cliente aún
    /// no aceptó la letra. El documento queda libre para otra cancelación.
    /// </summary>
    public record EliminarDocumentoLetraVentaCommand(string PlantaCode, string Number, string Item)
        : IRequest<LetraVentaResponse>;
}
