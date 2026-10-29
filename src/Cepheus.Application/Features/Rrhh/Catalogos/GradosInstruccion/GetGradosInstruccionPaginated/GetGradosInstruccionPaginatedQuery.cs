using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.GetGradosInstruccionPaginated
{
    public class GetGradosInstruccionPaginatedQuery : PagedRequest, IRequest<PagedResult<GradoInstruccionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}