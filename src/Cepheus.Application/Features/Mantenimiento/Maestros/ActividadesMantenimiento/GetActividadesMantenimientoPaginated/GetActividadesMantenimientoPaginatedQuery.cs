using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.GetActividadesMantenimientoPaginated
{
    public class GetActividadesMantenimientoPaginatedQuery : PagedRequest, IRequest<PagedResult<ActividadMantenimientoResponse>>
    {
        /// <summary>Busca por código de actividad o de actividad cubierta.</summary>
        public string? Search { get; set; }
        public string? ActividadCode { get; set; }
    }
}
