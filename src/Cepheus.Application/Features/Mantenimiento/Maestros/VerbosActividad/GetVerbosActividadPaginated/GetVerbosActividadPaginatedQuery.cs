using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.GetVerbosActividadPaginated
{
    public class GetVerbosActividadPaginatedQuery : PagedRequest, IRequest<PagedResult<VerboActividadResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
