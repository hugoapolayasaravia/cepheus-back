using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.CreateOTRMaquina
{
    public record CreateOTRMaquinaCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        string MaquinaCode,
        DateTime? FechaProceso,
        decimal Cantidad,
        decimal Horas
    ) : IRequest<OTRMaquinaResponse>;
}
