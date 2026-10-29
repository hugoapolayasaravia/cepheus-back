using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.Me
{
    /// <summary>
    /// No recibe Id: siempre resuelve contra el usuario autenticado actual
    /// (ICurrentUserService), leído de los claims del JWT.
    /// </summary>
    /// <param name="RoleIds">
    /// Opcional. Limita el menú y los permisos devueltos a estos roles (solo se consideran los
    /// roles que el usuario realmente tiene). Es una vista: la autorización de los endpoints
    /// sigue evaluando todos los roles del usuario.
    /// </param>
    public record GetMeQuery(int[]? RoleIds = null) : IRequest<MeResponse>;
}
