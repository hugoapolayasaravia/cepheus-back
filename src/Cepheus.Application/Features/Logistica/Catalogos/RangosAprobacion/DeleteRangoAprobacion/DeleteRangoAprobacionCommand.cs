using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.DeleteRangoAprobacion
{
    public record DeleteRangoAprobacionCommand(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode
    ) : IRequest;
}
