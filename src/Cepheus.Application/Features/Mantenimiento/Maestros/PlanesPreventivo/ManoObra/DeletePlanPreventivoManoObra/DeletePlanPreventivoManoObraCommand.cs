using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.DeletePlanPreventivoManoObra
{
    public record DeletePlanPreventivoManoObraCommand(string PlantaCode, string PlanCode, string TrabajadorCode) : IRequest;
}
