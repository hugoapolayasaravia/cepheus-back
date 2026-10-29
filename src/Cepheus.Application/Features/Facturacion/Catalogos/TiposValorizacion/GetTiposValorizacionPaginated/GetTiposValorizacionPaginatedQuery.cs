using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.GetTiposValorizacionPaginated
{
    public class GetTiposValorizacionPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoValorizacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
