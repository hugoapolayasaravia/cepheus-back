using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.GetPermissionById
{
    public record GetPermissionByIdQuery(int Id) : IRequest<PermissionResponse>;
}
