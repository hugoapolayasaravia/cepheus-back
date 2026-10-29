using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.GetHorariosPaginated
{
    public class GetHorariosPaginatedQuery : PagedRequest, IRequest<PagedResult<HorarioResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}