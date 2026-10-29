using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.TiposDocumento.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.GetTiposDocumentoPaginated
{
    public class GetTiposDocumentoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoDocumentoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
