using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.GetTecnicosPaginated
{
    public class GetTecnicosPaginatedQuery : PagedRequest, IRequest<PagedResult<TecnicoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
