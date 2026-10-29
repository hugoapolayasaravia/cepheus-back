using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.GetPlanPreventivoByKey
{
    public record GetPlanPreventivoByKeyQuery(string PlantaCode, string Code) : IRequest<PlanPreventivoResponse>;
}
