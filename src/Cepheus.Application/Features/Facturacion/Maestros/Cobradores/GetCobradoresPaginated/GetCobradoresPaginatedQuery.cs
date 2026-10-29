using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.GetCobradoresPaginated
{
    public class GetCobradoresPaginatedQuery : PagedRequest, IRequest<PagedResult<CobradorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
