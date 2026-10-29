using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.ToggleArticuloStatus
{
    public record ToggleArticuloStatusCommand(string Code) : IRequest<bool>;
}
