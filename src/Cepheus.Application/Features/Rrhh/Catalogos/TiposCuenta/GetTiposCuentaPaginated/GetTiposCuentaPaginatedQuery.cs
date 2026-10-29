using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.GetTiposCuentaPaginated
{
    public class GetTiposCuentaPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoCuentaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}