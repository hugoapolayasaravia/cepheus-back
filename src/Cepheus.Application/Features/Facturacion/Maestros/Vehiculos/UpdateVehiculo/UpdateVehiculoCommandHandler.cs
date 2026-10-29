using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using Cepheus.Domain.Facturacion.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.UpdateVehiculo
{
    public class UpdateVehiculoCommandHandler : IRequestHandler<UpdateVehiculoCommand, VehiculoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateVehiculoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<VehiculoResponse> Handle(UpdateVehiculoCommand request, CancellationToken cancellationToken)
        {
            var transportistaCode = request.TransportistaCode.Trim().ToUpperInvariant();
            var vehicleType = TipoVehiculoParser.ParseOrNotFound(request.VehicleType);
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Facturacion.Maestros.Vehiculos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(v =>
                    v.TransportistaCode == transportistaCode &&
                    v.VehicleType == vehicleType &&
                    v.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Vehículo {transportistaCode}/{vehicleType}/{code} no encontrado.");
            }

            var vehiculo = new VehiculoVenta
            {
                TransportistaCode = transportistaCode,
                VehicleType = vehicleType,
                Code = code,
                LicensePlate = request.LicensePlate.Trim().ToUpperInvariant(),
                Brand = string.IsNullOrWhiteSpace(request.Brand) ? null : request.Brand.Trim(),
                Model = string.IsNullOrWhiteSpace(request.Model) ? null : request.Model.Trim(),
                ChoferCode = string.IsNullOrWhiteSpace(request.ChoferCode) ? null : request.ChoferCode.Trim().ToUpperInvariant(),
                Capacity = request.Capacity,
                Suple = request.Suple,
                LengthM = request.LengthM,
                WidthM = request.WidthM,
                HeightM = request.HeightM,
                Telescopic = request.Telescopic,
                WithoutSuple = request.WithoutSuple,
                WithSuple = request.WithSuple,
                CubicWithoutSuple = request.CubicWithoutSuple,
                CubicWithSuple = request.CubicWithSuple,
                MtcInternalCode = string.IsNullOrWhiteSpace(request.MtcInternalCode) ? null : request.MtcInternalCode.Trim(),
                VehicularConfiguration = string.IsNullOrWhiteSpace(request.VehicularConfiguration) ? null : request.VehicularConfiguration.Trim(),
                PlanillaCode = string.IsNullOrWhiteSpace(request.PlanillaCode) ? null : request.PlanillaCode.Trim(),
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),

                // La aprobación no se edita desde este comando: se conserva lo existente.
                ApprovedBy = current.ApprovedBy,
                ApprovedAt = current.ApprovedAt,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Maestros.Vehiculos.Update(vehiculo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El vehículo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateVehiculo.CreateVehiculoCommandHandler.Map(vehiculo);
        }
    }
}
