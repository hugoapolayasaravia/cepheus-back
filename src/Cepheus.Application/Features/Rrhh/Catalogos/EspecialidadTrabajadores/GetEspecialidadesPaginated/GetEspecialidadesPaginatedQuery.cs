using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.GetEspecialidadesPaginated
{
    public class GetEspecialidadesPaginatedQuery : PagedRequest, IRequest<PagedResult<EspecialidadTrabajadorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}