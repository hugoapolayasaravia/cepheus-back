using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.GetEquiposPaginated
{
    public class GetEquiposPaginatedQuery : PagedRequest, IRequest<PagedResult<EquipoResponse>>
    {
        public string? Search { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public int? Nivel { get; set; }
        public bool? IsActive { get; set; }
    }
}
