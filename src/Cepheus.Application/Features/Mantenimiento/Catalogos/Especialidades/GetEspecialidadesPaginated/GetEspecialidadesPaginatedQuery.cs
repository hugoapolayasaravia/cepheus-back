using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.GetEspecialidadesPaginated
{
    public class GetEspecialidadesPaginatedQuery : PagedRequest, IRequest<PagedResult<EspecialidadResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
