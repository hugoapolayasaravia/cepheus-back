using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.GetEstadosTrabajadorPaginated
{
    public class GetEstadosTrabajadorPaginatedQuery : PagedRequest, IRequest<PagedResult<EstadoTrabajadorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}