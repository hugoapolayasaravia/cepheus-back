using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.ToggleSubmoduloStatus
{
    public record ToggleSubmoduloStatusCommand(int Id) : IRequest<bool>;
}
