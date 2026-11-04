using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.ManoObra.GetPlanPreventivoManoObraByPlan
{
    public record GetPlanPreventivoManoObraByPlanQuery(string PlantaCode, string PlanCode) : IRequest<List<PlanPreventivoManoObraResponse>>;
}
