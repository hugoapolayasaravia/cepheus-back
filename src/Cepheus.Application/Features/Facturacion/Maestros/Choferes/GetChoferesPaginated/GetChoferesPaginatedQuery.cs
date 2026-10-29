using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.GetChoferesPaginated
{
    public class GetChoferesPaginatedQuery : PagedRequest, IRequest<PagedResult<ChoferResponse>>
    {
        public string? Search { get; set; }
        public string? TransportistaCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
