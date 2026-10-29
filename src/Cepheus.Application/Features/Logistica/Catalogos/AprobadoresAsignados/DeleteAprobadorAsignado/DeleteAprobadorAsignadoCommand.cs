using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.DeleteAprobadorAsignado
{
    public record DeleteAprobadorAsignadoCommand(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode,
        string TrabajadorCode
    ) : IRequest;
}
