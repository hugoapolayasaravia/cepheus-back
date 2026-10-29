using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.GetTitulosPaginated
{
    public class GetTitulosPaginatedQuery : PagedRequest, IRequest<PagedResult<TituloResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}