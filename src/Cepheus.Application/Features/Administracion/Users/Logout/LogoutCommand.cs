using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest;
}
