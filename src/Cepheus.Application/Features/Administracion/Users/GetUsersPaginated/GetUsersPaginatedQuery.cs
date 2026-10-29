using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.GetUsersPaginated
{
    public class GetUsersPaginatedQuery : PagedRequest, IRequest<PagedResult<UserResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
