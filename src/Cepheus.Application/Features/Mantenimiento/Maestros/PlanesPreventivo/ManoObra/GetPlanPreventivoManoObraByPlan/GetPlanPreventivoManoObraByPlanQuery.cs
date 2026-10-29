using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.GetPlanPreventivoManoObraByPlan
{
    public record GetPlanPreventivoManoObraByPlanQuery(string PlantaCode, string PlanCode) : IRequest<List<PlanPreventivoManoObraResponse>>;
}
