using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccion
{
    /// <summary>
    /// Cambia el rango de fechas de un lote Pendiente. Si las fechas cambian, el lote se regenera con
    /// los documentos del nuevo rango (legacy: "regenerar las detracciones para este lote").
    /// </summary>
    public record UpdateDetraccionCommand(
        string Year,
        string Period,
        string LotNumber,
        DateTime StartDate,
        DateTime EndDate
    ) : IRequest<DetraccionResponse>, IDetraccionPeriodRequest;
}
