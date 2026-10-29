using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresAsignadosPaginated
{
    public class GetAprobadoresAsignadosPaginatedQuery : PagedRequest, IRequest<PagedResult<AprobadorAsignadoResponse>>
    {
        public string? TrabajadorCode { get; set; }
        public string? TipoTransaccionCode { get; set; }
        public string? UnidadNegocioCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
