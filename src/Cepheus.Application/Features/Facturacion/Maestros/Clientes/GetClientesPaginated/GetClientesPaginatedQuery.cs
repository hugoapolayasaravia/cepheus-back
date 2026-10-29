using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.GetClientesPaginated
{
    public class GetClientesPaginatedQuery : PagedRequest, IRequest<PagedResult<ClienteResponse>>
    {
        public string? Search { get; set; }
        public string? TipoClienteCode { get; set; }
        public string? ClasificacionClienteCode { get; set; }
        public string? Estado { get; set; }
    }
}
