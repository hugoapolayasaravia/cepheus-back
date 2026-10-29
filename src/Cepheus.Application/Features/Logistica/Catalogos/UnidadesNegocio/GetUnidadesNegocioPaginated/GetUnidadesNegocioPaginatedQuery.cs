using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.GetUnidadesNegocioPaginated
{
    public class GetUnidadesNegocioPaginatedQuery : PagedRequest, IRequest<PagedResult<UnidadNegocioResponse>>
    {
        public string? Search { get; set; }
        public string? ParentCode { get; set; }
        public bool? IsActive { get; set; }
    }
}