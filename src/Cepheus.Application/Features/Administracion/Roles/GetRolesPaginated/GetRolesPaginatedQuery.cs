using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.GetRolesPaginated
{
    public class GetRolesPaginatedQuery : PagedRequest, IRequest<PagedResult<RoleResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }

}
