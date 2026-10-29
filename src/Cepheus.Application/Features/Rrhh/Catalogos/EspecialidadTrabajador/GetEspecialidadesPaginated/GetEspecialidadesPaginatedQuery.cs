using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.GetEspecialidadesPaginated
{
    public class GetEspecialidadesPaginatedQuery : PagedRequest, IRequest<PagedResult<EspecialidadTrabajadorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}