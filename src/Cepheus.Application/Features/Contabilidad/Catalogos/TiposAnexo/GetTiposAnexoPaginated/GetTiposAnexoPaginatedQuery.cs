using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.GetTiposAnexoPaginated
{
    public class GetTiposAnexoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoAnexoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
