using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.GetAlergiasPaginated
{
    public class GetAlergiasPaginatedQuery : PagedRequest, IRequest<PagedResult<AlergiaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}