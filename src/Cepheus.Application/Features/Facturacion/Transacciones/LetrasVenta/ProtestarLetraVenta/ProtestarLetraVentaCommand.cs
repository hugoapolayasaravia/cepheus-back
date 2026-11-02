using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.ProtestarLetraVenta
{
    /// <summary>
    /// Protesta la letra (legacy ue_set_btn_protestar): vuelve a ser una deuda por cobrar (Facturado) y se
    /// recalcula la cancelación que la originó y sus documentos. No se puede protestar si ya se aplicó en
    /// otra cancelación.
    /// </summary>
    public record ProtestarLetraVentaCommand(string PlantaCode, string Number, byte[] RowVersion)
        : IRequest<LetraVentaResponse>;
}
