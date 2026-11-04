using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.GetPlanPreventivoMaterialByPlan
{
    public record GetPlanPreventivoMaterialByPlanQuery(string PlantaCode, string PlanCode) : IRequest<List<PlanPreventivoMaterialResponse>>;
}
