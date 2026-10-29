using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.GetTiposTrabajadorPaginated
{
    public class GetTiposTrabajadorPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoTrabajadorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}