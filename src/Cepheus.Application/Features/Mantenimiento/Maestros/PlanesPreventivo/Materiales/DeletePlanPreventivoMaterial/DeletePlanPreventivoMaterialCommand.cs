using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.DeletePlanPreventivoMaterial
{
    public record DeletePlanPreventivoMaterialCommand(string PlantaCode, string PlanCode, string ArticuloCode) : IRequest;
}
