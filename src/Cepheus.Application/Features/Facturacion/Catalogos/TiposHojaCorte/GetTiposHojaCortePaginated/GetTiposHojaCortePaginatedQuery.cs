using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.GetTiposHojaCortePaginated
{
    public class GetTiposHojaCortePaginatedQuery : PagedRequest, IRequest<PagedResult<TipoHojaCorteResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
