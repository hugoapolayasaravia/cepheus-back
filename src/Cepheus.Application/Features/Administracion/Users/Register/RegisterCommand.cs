using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.Register
{
    /// <summary>
    /// Bootstrap: crea el PRIMER usuario del sistema junto con el rol "Administrador"
    /// y se lo asigna. Solo funciona si la tabla Users está vacía (ver
    /// RegisterCommandHandler). Una vez usado, queda bloqueado para siempre:
    /// el resto de usuarios se crea vía CreateUser (protegido, admin-only).
    /// </summary>
    public record RegisterCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName
    ) : IRequest<UserResponse>;

}
