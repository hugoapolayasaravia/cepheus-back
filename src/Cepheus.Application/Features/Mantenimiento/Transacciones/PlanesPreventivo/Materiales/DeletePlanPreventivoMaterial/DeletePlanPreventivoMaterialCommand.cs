using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.DeletePlanPreventivoMaterial
{
    public record DeletePlanPreventivoMaterialCommand(string PlantaCode, string PlanCode, string ArticuloCode) : IRequest;
}
