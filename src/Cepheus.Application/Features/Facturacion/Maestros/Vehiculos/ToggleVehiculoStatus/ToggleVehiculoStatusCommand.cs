using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.ToggleVehiculoStatus
{
    public record ToggleVehiculoStatusCommand(string TransportistaCode, string VehicleType, string Code) : IRequest<bool>;
}
