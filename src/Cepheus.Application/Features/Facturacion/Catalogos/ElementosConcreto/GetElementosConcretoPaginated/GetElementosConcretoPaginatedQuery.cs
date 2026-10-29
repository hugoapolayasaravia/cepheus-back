using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.GetElementosConcretoPaginated
{
    public class GetElementosConcretoPaginatedQuery : PagedRequest, IRequest<PagedResult<ElementoConcretoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
