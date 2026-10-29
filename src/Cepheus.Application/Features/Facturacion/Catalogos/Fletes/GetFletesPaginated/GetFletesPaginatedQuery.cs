using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFletesPaginated
{
    public class GetFletesPaginatedQuery : PagedRequest, IRequest<PagedResult<FleteResponse>>
    {
        public string? Search { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
    }
}
