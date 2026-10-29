using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GenerarAsientoConsumo
{
    /// <summary>Genera UN asiento con las líneas de vale pendientes de la planta y el rango de fechas (período: mes de la fecha final).</summary>
    public record GenerarAsientoConsumoCommand(string PlantaCode, DateTime FechaDesde, DateTime FechaHasta)
        : IRequest<AsientoConsumoResultResponse>;
}
