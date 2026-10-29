using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.GetTiposSangrePaginated
{
    public class GetTiposSangrePaginatedQuery : PagedRequest, IRequest<PagedResult<TipoSangreResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}