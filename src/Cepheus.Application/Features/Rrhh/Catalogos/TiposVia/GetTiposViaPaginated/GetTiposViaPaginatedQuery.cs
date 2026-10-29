using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.GetTiposViaPaginated
{
    public class GetTiposViaPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoViaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}