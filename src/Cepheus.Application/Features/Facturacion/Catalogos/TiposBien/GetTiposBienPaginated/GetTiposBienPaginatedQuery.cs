using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.GetTiposBienPaginated
{
    public class GetTiposBienPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoBienResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
