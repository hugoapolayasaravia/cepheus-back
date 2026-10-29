using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.GetAlturasLosaPaginated
{
    public class GetAlturasLosaPaginatedQuery : PagedRequest, IRequest<PagedResult<AlturaLosaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
