using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.GetPlanPreventivoByKey
{
    public record GetPlanPreventivoByKeyQuery(string PlantaCode, string Code) : IRequest<PlanPreventivoResponse>;
}
