using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.UpdatePlanArticulo
{
    public record UpdatePlanArticuloCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<PlanArticuloResponse>;
}