using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.GetTiposContratoPaginated
{
    public class GetTiposContratoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoContratoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}