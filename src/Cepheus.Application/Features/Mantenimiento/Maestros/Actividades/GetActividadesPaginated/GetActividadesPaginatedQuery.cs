using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.GetActividadesPaginated
{
    public class GetActividadesPaginatedQuery : PagedRequest, IRequest<PagedResult<ActividadResponse>>
    {
        public string? Search { get; set; }
        public string? VerboActividadCode { get; set; }
        public string? ObjetoActividadCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
