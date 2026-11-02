using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.FormasPagoCancelacion.GetFormasPagoCancelacionPaginated
{
    public class GetFormasPagoCancelacionPaginatedQuery : PagedRequest, IRequest<PagedResult<FormaPagoCancelacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
