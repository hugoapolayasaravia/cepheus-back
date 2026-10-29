using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.SendPasswordLink
{
    /// <summary>
    /// Envía al usuario un enlace de un solo uso para que cree (o cambie) su contraseña.
    /// Sirve para reenviar una invitación que venció y para ayudar a quien perdió su acceso.
    /// </summary>
    public record SendPasswordLinkCommand(int Id) : IRequest;
}
