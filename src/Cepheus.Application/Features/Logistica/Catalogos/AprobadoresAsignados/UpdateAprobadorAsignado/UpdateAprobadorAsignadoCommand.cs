using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.UpdateAprobadorAsignado
{
    /// <summary>Solo permite corregir Suplente/Superior — la clave (incluido el titular) no se edita.</summary>
    public record UpdateAprobadorAsignadoCommand(
        string NivelCode,
        string TipoTransaccionCode,
        string UnidadNegocioCode,
        string MonedaCode,
        string TrabajadorCode,
        string? SuplenteTrabajadorCode,
        string? SuperiorTrabajadorCode,
        byte[] RowVersion
    ) : IRequest<AprobadorAsignadoResponse>;
}
