using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common.Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.GetNotasCompraPaginated
{
    public class GetNotasCompraPaginatedQuery : PagedRequest, IRequest<PagedResult<NotaCompraResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}