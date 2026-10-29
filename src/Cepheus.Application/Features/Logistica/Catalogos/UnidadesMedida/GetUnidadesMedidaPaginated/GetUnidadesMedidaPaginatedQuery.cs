using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.GetUnidadesMedidaPaginated
{
    public class GetUnidadesMedidaPaginatedQuery : PagedRequest, IRequest<PagedResult<UnidadMedidaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}