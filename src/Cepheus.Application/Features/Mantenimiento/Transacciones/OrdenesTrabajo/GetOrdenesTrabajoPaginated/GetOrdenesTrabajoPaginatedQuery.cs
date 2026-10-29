using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.GetOrdenesTrabajoPaginated
{
    public class GetOrdenesTrabajoPaginatedQuery : PagedRequest, IRequest<PagedResult<OrdenTrabajoResponse>>
    {
        public string? Search { get; set; }
        public string? PlantaCode { get; set; }
        public string? Estado { get; set; }
        public string? EquipoCode { get; set; }
        public string? ResponsableCode { get; set; }
        public string? PrioridadCode { get; set; }
        public DateTime? FechaServicioDesde { get; set; }
        public DateTime? FechaServicioHasta { get; set; }
    }
}
