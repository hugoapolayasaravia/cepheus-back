using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CancelarDetraccion
{
    /// <summary>Registra la cancelación de tesorería de un lote Emitido: fecha y número de comprobante de pago.</summary>
    public record CancelarDetraccionCommand(
        string Year,
        string Period,
        string LotNumber,
        DateTime CancellationDate,
        string PaymentReceiptNumber
    ) : IRequest<DetraccionResponse>;
}
