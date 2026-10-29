using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.TogglePlanArticuloStatus
{
    public record TogglePlanArticuloStatusCommand(string Code) : IRequest<bool>;
}