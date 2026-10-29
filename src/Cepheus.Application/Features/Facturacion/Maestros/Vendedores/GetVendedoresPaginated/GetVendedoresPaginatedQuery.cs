using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.GetVendedoresPaginated
{
    public class GetVendedoresPaginatedQuery : PagedRequest, IRequest<PagedResult<VendedorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
