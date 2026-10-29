using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.GetPlanesPreventivoPaginated
{
    /// <summary>Filtros del listado legacy Logi_sp_Listado_Man_Preventivo (planta, código, equipo, responsable).</summary>
    public class GetPlanesPreventivoPaginatedQuery : PagedRequest, IRequest<PagedResult<PlanPreventivoResponse>>
    {
        /// <summary>Busca por descripción o nombre de equipo.</summary>
        public string? Search { get; set; }
        public string? PlantaCode { get; set; }
        public string? Code { get; set; }
        /// <summary>Coincidencia parcial (el legacy usaba LIKE %equipo%).</summary>
        public string? EquipoCode { get; set; }
        public string? ResponsableCode { get; set; }
    }
}
