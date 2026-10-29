using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.CreateOTResponsable
{
    public record CreateOTResponsableCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        DateTime? FechaProceso,
        string TrabajadorCode,
        decimal TiempoProceso,
        decimal Basico,
        decimal CostoTotal
    ) : IRequest<OTResponsableResponse>;
}
