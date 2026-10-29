using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.GetCargosPaginated
{
    public class GetCargosPaginatedQuery : PagedRequest, IRequest<PagedResult<CargoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}