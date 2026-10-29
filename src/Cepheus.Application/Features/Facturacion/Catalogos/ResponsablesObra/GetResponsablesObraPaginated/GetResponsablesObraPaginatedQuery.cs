using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.GetResponsablesObraPaginated
{
    public class GetResponsablesObraPaginatedQuery : PagedRequest, IRequest<PagedResult<ResponsableObraResponse>>
    {
        public string? ClienteCode { get; set; }
        public string? ObraCode { get; set; }
        public string? Search { get; set; }
    }
}
