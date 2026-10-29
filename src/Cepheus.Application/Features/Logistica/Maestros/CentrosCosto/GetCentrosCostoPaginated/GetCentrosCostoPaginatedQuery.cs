using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.GetCentrosCostoPaginated
{
    public class GetCentrosCostoPaginatedQuery : PagedRequest, IRequest<PagedResult<CentroCostoResponse>>
    {
        public string? Search { get; set; }
        public string? PlantaCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
