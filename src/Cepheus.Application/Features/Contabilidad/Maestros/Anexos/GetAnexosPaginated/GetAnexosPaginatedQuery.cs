using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.GetAnexosPaginated
{
    public class GetAnexosPaginatedQuery : PagedRequest, IRequest<PagedResult<AnexoResponse>>
    {
        public string? Search { get; set; }
        public string? TipoAnexoCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
