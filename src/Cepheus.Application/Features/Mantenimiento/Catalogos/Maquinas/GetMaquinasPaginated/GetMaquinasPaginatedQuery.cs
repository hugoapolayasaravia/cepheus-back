using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.GetMaquinasPaginated
{
    public class GetMaquinasPaginatedQuery : PagedRequest, IRequest<PagedResult<MaquinaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
