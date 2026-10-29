using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.GetPlanPreventivoMaterialByPlan
{
    public record GetPlanPreventivoMaterialByPlanQuery(string PlantaCode, string PlanCode) : IRequest<List<PlanPreventivoMaterialResponse>>;
}
