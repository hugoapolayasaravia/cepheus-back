using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.GetObjetosActividadPaginated
{
    public class GetObjetosActividadPaginatedQuery : PagedRequest, IRequest<PagedResult<ObjetoActividadResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
