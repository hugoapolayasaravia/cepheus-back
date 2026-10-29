using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.GetProveedoresPaginated
{
    public class GetProveedoresPaginatedQuery : PagedRequest, IRequest<PagedResult<ProveedorResponse>>
    {
        public string? Search { get; set; }
        public ProviderType? ProviderType { get; set; }
        public ProviderOrigin? Origin { get; set; }
        public bool? IsActive { get; set; }
    }
}
