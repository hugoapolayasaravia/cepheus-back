using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.GetModosPagoPaginated
{
    public class GetModosPagoPaginatedQuery : PagedRequest, IRequest<PagedResult<ModoPagoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}