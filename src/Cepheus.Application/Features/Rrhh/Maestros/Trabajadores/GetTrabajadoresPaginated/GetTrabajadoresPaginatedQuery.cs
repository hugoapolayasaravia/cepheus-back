using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.GetTrabajadoresPaginated
{
    public class GetTrabajadoresPaginatedQuery : PagedRequest, IRequest<PagedResult<TrabajadorResponse>>
    {
        public string? Search { get; set; }
    }
}