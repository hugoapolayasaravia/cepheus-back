using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CreateDetraccion
{
    /// <summary>
    /// Crea un lote de detracción: el número de lote es el siguiente correlativo del año y periodo, y los
    /// documentos, ítems y montos se generan de las compras procesadas entre StartDate y EndDate.
    /// </summary>
    public record CreateDetraccionCommand(
        string Year,
        string Period,
        DateTime StartDate,
        DateTime EndDate
    ) : IRequest<DetraccionResponse>, IDetraccionPeriodRequest;
}
