using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.GetNivelesTrabajadorPaginated
{
    public class GetNivelesTrabajadorPaginatedQuery : PagedRequest, IRequest<PagedResult<NivelTrabajadorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}