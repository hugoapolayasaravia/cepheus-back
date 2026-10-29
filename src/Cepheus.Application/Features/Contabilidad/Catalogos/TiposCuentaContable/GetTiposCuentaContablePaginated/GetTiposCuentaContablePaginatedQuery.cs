using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.GetTiposCuentaContablePaginated
{
    public class GetTiposCuentaContablePaginatedQuery : PagedRequest, IRequest<PagedResult<TipoCuentaContableResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
