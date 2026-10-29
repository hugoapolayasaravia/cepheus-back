using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.GetRoleById
{
    public record GetRoleByIdQuery(int Id) : IRequest<RoleResponse>;
}
