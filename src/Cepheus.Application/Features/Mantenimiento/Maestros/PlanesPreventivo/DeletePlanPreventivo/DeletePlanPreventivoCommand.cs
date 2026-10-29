using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.DeletePlanPreventivo
{
    /// <summary>Elimina el plan junto con su mano de obra y materiales (cascada, como en el legacy).</summary>
    public record DeletePlanPreventivoCommand(string PlantaCode, string Code) : IRequest;
}
