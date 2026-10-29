using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.GetUnidadesHorometroPaginated
{
    public class GetUnidadesHorometroPaginatedQuery : PagedRequest, IRequest<PagedResult<UnidadHorometroResponse>>
    {
        /// <summary>Busca por código o nombre del subcentro de costo.</summary>
        public string? Search { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public string? PlantaAfectadaCode { get; set; }
        public string? EquipoCode { get; set; }
        public EstadoEquipo? Estado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}
