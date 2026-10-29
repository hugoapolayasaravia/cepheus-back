using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Plantas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.GetPlantasPaginated
{
    public class GetPlantasPaginatedQuery : PagedRequest, IRequest<PagedResult<PlantaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsProductionPlant { get; set; }
    }
}
