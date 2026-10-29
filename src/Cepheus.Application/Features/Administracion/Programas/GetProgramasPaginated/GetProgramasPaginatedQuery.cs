using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Programas.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.GetProgramasPaginated
{
    public class GetProgramasPaginatedQuery : PagedRequest, IRequest<PagedResult<ProgramaResponse>>
    {
        public int? SubmoduloId { get; set; }
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }

}
