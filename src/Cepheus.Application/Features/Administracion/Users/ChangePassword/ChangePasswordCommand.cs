using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.ChangePassword
{
    /// <summary>
    /// Self-service: Id debe coincidir con el usuario autenticado (validado en el
    /// Handler vía ICurrentUserService). Un admin reseteando la contraseña de OTRO
    /// usuario sin conocer la actual es un caso distinto, no cubierto acá — se
    /// diseña cuando exista el sistema de permisos (Role/Permission).
    /// </summary>
    public record ChangePasswordCommand(
        int Id,
        string CurrentPassword,
        string NewPassword
    ) : IRequest;
}
