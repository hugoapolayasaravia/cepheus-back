using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.GetRangosAprobacionPaginated
{
    public class GetRangosAprobacionPaginatedQuery : PagedRequest, IRequest<PagedResult<RangoAprobacionResponse>>
    {
        public string? TipoTransaccionCode { get; set; }
        public string? UnidadNegocioCode { get; set; }
        public string? MonedaCode { get; set; }
        public string? NivelCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
