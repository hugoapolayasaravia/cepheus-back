using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.GetRangoAprobacionByKey
{
    public record GetRangoAprobacionByKeyQuery(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode
    ) : IRequest<RangoAprobacionResponse>;
}
