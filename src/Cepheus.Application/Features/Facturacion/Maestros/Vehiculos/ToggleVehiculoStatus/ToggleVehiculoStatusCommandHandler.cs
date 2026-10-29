using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.ToggleVehiculoStatus
{
    public class ToggleVehiculoStatusCommandHandler : IRequestHandler<ToggleVehiculoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleVehiculoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleVehiculoStatusCommand request, CancellationToken cancellationToken)
        {
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var vehicleType = TipoVehiculoParser.ParseOrNotFound(request.VehicleType);
            var code = request.Code.Trim().ToUpperInvariant();

            var vehiculo = await _uow.Facturacion.Maestros.Vehiculos.Query()
                .FirstOrDefaultAsync(v =>
                    v.TransportistaCode == transportistaCode &&
                    v.VehicleType == vehicleType &&
                    v.Code == code, cancellationToken);

            if (vehiculo is null)
            {
                throw new KeyNotFoundException($"Vehículo {transportistaCode}/{vehicleType}/{code} no encontrado.");
            }

            vehiculo.IsActive = !vehiculo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return vehiculo.IsActive;
        }
    }
}
