using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.Authenticate
{
    public record AuthenticateCommand(
         string Email,
         string Password
     ) : IRequest<AuthenticateResponse>;

}
