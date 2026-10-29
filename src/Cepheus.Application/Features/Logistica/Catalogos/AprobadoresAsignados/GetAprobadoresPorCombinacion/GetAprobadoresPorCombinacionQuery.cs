using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion
{
    public record GetAprobadoresPorCombinacionQuery(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode
    ) : IRequest<List<AprobadorAsignadoResponse>>;
}
