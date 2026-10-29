using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.GetLugaresEnvioPaginated
{
    public class GetLugaresEnvioPaginatedQuery : PagedRequest, IRequest<PagedResult<LugarEnvioResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}