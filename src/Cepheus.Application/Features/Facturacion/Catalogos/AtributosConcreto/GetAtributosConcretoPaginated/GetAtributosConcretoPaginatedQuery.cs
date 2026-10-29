using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.GetAtributosConcretoPaginated
{
    public class GetAtributosConcretoPaginatedQuery : PagedRequest, IRequest<PagedResult<AtributoConcretoResponse>>
    {
        public string? Search { get; set; }
        public string? AttributeType { get; set; }
        public bool? IsActive { get; set; }
    }
}
