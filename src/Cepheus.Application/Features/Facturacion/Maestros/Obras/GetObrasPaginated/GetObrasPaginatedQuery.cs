using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.GetObrasPaginated
{
    public class GetObrasPaginatedQuery : PagedRequest, IRequest<PagedResult<ObraResponse>>
    {
        public string? Search { get; set; }
        public string? ClienteCode { get; set; }
        public string? VendedorCode { get; set; }
        public string? CobradorCode { get; set; }
        public string? Estado { get; set; }
    }
}
