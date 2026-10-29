using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.ToggleUbigeoStatus
{
    public record ToggleUbigeoStatusCommand(string Code) : IRequest<bool>;
}
