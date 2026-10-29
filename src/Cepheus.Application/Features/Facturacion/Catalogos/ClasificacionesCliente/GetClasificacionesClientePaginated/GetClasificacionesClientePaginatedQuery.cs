using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.GetClasificacionesClientePaginated
{
    public class GetClasificacionesClientePaginatedQuery : PagedRequest, IRequest<PagedResult<ClasificacionClienteResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
