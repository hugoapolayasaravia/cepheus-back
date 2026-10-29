using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.GetVehiculoByCode
{
    public record GetVehiculoByCodeQuery(string TransportistaCode, string VehicleType, string Code) : IRequest<VehiculoResponse>;
}
