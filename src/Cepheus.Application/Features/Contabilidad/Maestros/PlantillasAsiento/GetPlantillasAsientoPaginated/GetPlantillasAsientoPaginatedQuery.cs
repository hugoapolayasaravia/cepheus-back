using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.GetPlantillasAsientoPaginated
{
    public class GetPlantillasAsientoPaginatedQuery : PagedRequest, IRequest<PagedResult<PlantillaAsientoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public ProcesoAsiento? Process { get; set; }
        public string? TipoAsientoCode { get; set; }
    }
}
