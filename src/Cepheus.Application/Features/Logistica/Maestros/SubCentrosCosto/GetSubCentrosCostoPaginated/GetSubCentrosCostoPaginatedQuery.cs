using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.GetSubCentrosCostoPaginated
{
    public class GetSubCentrosCostoPaginatedQuery : PagedRequest, IRequest<PagedResult<SubCentroCostoResponse>>
    {
        public string? Search { get; set; }
        public string? CentroCostoCode { get; set; }
        public string? PlantaCode { get; set; }

        /// <summary>Cadena vacía ("") filtra los subcentros raíz (sin padre).</summary>
        public string? ParentCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
