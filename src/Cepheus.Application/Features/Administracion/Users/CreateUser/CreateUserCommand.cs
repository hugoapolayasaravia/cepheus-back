using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.CreateUser
{
    /// <summary>
    /// Si Password viene vacío, el usuario se crea en modo invitación: recibe un correo con un enlace
    /// para definir su propia contraseña. Si viene con valor, se usa tal cual (comportamiento anterior).
    /// </summary>
    public record CreateUserCommand(
    string Email,
    string? Password,
    string FirstName,
    string LastName
) : IRequest<UserResponse>;


}
