using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.GetOportunidadesPaginated
{
    public class GetOportunidadesPaginatedQuery : PagedRequest, IRequest<PagedResult<OportunidadResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
