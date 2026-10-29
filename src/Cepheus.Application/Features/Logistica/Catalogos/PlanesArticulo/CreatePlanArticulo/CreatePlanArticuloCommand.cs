using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.CreatePlanArticulo
{
    public record CreatePlanArticuloCommand(
        string Name
    ) : IRequest<PlanArticuloResponse>;
}