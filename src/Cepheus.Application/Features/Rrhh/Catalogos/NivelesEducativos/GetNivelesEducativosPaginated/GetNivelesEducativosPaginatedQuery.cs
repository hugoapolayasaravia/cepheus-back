using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.GetNivelesEducativosPaginated
{
    public class GetNivelesEducativosPaginatedQuery : PagedRequest, IRequest<PagedResult<NivelEducativoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}