using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMaquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMaquinas.CreateOTRMaquina
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
