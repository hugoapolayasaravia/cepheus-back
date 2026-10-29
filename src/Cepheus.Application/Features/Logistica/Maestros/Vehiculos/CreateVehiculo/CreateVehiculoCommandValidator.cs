using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.CreateVehiculo
{
    public class CreateVehiculoCommandValidator : AbstractValidator<CreateVehiculoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateVehiculoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TransportistaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El transportista es obligatorio.")
                .MustAsync(TransportistaExists).WithMessage("El transportista indicado no existe.");

            RuleFor(x => x.LicensePlate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La placa es obligatoria.")
                .MaximumLength(10).WithMessage("La placa no puede exceder los 10 caracteres.")
                .MustAsync(BeUniquePlate).WithMessage("Ya existe un vehículo con esa placa.");

            RuleFor(x => x.VehicleCategory).MaximumLength(10);
            RuleFor(x => x.VehicleType).MaximumLength(30);
            RuleFor(x => x.Brand).MaximumLength(50);
            RuleFor(x => x.Model).MaximumLength(50);

            RuleFor(x => x.ManufactureYear)
                .InclusiveBetween((short)1900, (short)(DateTime.UtcNow.Year + 1))
                .WithMessage("El año de fabricación no es válido.")
                .When(x => x.ManufactureYear.HasValue);

            RuleFor(x => x.EngineNumber).MaximumLength(50);
            RuleFor(x => x.ChassisNumber).MaximumLength(50);
            RuleFor(x => x.Color).MaximumLength(30);

            RuleFor(x => x.CargoCapacityKg).GreaterThanOrEqualTo(0).When(x => x.CargoCapacityKg.HasValue);
            RuleFor(x => x.LengthM).GreaterThanOrEqualTo(0).When(x => x.LengthM.HasValue);
            RuleFor(x => x.WidthM).GreaterThanOrEqualTo(0).When(x => x.WidthM.HasValue);
            RuleFor(x => x.HeightM).GreaterThanOrEqualTo(0).When(x => x.HeightM.HasValue);

            RuleFor(x => x.VehicularCertificateNumber).MaximumLength(50);
            RuleFor(x => x.CirculationCardNumber).MaximumLength(50);
            RuleFor(x => x.VehicularConfiguration).MaximumLength(30);
            RuleFor(x => x.Observations).MaximumLength(500);
        }

        private async Task<bool> TransportistaExists(string code, CancellationToken ct)
            => await _uow.Logistica.Maestros.Transportistas.Query().AnyAsync(t => t.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniquePlate(string plate, CancellationToken ct)
            => !await _uow.Logistica.Maestros.Vehiculos.Query().AnyAsync(v => v.LicensePlate == plate.Trim().ToUpper(), ct);
    }
}
