using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.GetVehiculosPaginated
{
    public class GetVehiculosPaginatedQuery : PagedRequest, IRequest<PagedResult<VehiculoResponse>>
    {
        public string? Search { get; set; }
        public string? TransportistaCode { get; set; }
        public string? VehicleType { get; set; }
        public bool? IsActive { get; set; }
    }
}
