using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Submodulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Submodulos.GetSubmodulosPaginated
{
    public class GetSubmodulosPaginatedQuery : PagedRequest, IRequest<PagedResult<SubmoduloResponse>>
    {
        public int? ModuloId { get; set; }
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }

}

