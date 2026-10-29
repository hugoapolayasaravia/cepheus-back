using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.GetPlanArticuloByCode
{
    public record GetPlanArticuloByCodeQuery(string Code) : IRequest<PlanArticuloResponse>;
}