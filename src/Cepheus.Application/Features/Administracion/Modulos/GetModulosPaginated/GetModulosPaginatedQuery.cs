using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Modulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.GetModulosPaginated
{
    public class GetModulosPaginatedQuery : PagedRequest, IRequest<PagedResult<ModuloResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }

}
