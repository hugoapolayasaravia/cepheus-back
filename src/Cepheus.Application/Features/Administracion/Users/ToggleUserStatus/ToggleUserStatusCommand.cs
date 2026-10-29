using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.ToggleUserStatus
{
    public record ToggleUserStatusCommand(int Id) : IRequest<bool>;
}
