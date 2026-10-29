using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.ToggleElementoConcretoStatus
{
    public record ToggleElementoConcretoStatusCommand(string Code) : IRequest<bool>;
}
