using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.CreateRangoAprobacion
{
    public record CreateRangoAprobacionCommand(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode,
        decimal ImporteMinimo,
        decimal ImporteMaximo,
        decimal ImporteAcumuladoDiario,
        decimal ImporteAcumuladoMensual,
        decimal? PorcentajeTotal
    ) : IRequest<RangoAprobacionResponse>;
}
