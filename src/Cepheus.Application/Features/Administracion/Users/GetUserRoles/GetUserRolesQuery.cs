using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.GetUserRoles
{
    public record GetUserRolesQuery(int UserId) : IRequest<List<RoleResponse>>;
}
