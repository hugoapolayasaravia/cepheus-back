using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.Refresh
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthenticateResponse>;
}
