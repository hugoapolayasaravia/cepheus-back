using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.CreateAprobadorAsignado
{
    public record CreateAprobadorAsignadoCommand(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode,
        string TrabajadorCode,
        string? SuplenteTrabajadorCode,
        string? SuperiorTrabajadorCode
    ) : IRequest<AprobadorAsignadoResponse>;
}
