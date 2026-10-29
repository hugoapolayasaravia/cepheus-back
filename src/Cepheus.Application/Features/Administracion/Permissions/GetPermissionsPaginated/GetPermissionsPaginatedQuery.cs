using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.GetPermissionsPaginated
{
    public class GetPermissionsPaginatedQuery : PagedRequest, IRequest<PagedResult<PermissionResponse>>
    {
        public int? ProgramaId { get; set; }
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }

}
