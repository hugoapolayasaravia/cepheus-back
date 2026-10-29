using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Compradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Compradores.GetCompradoresPaginated
{
    public class GetCompradoresPaginatedQuery : PagedRequest, IRequest<PagedResult<CompradorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}