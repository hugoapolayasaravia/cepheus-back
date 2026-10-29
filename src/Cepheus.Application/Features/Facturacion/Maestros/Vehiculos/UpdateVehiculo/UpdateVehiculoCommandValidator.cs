using System.Linq.Expressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.UpdateVehiculo
{
    public class UpdateVehiculoCommandValidator : AbstractValidator<UpdateVehiculoCommand>
    {
        private const decimal MaxMeasure = 9999999999.99m;

        private readonly IUnitOfWork _uow;

        public UpdateVehiculoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TransportistaCode)
                .NotEmpty().WithMessage("El transportista es obligatorio.")
                .Length(4).WithMessage("El código de transportista debe tener 4 caracteres.");

            RuleFor(x => x.VehicleType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de vehículo es obligatorio.")
                .Must(t => TipoVehiculoParser.TryParse(t, out _))
                .WithMessage($"El tipo de vehículo debe ser uno de: {string.Join(", ", TipoVehiculoParser.ValidNames)}.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del vehículo es obligatorio.")
                .Length(4).WithMessage("El código del vehículo debe tener 4 caracteres.");

            RuleFor(x => x.LicensePlate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La placa es obligatoria.")
                .MaximumLength(10).WithMessage("La placa no puede exceder los 10 caracteres.")
                .MustAsync(BeUniquePlate).WithMessage("Ya existe un vehículo con esa placa para el transportista.");

            RuleFor(x => x.Brand).MaximumLength(50).WithMessage("La marca no puede exceder los 50 caracteres.");
            RuleFor(x => x.Model).MaximumLength(50).WithMessage("El modelo no puede exceder los 50 caracteres.");

            RuleFor(x => x.ChoferCode)
                .MustAsync(ChoferExistsForTransportista)
                .WithMessage("El chofer indicado no existe para ese transportista.")
                .When(x => !string.IsNullOrWhiteSpace(x.ChoferCode));

            AddMeasureRule(x => x.Capacity, "La capacidad");
            AddMeasureRule(x => x.Suple, "El suple");
            AddMeasureRule(x => x.LengthM, "El largo");
            AddMeasureRule(x => x.WidthM, "El ancho");
            AddMeasureRule(x => x.HeightM, "El alto");
            AddMeasureRule(x => x.Telescopic, "El valor telescópico");
            AddMeasureRule(x => x.WithoutSuple, "El valor sin suple");
            AddMeasureRule(x => x.WithSuple, "El valor con suple");
            AddMeasureRule(x => x.CubicWithoutSuple, "El cubicaje sin suple");
            AddMeasureRule(x => x.CubicWithSuple, "El cubicaje con suple");

            RuleFor(x => x.MtcInternalCode).MaximumLength(20).WithMessage("El código interno del MTC no puede exceder los 20 caracteres.");
            RuleFor(x => x.VehicularConfiguration).MaximumLength(30).WithMessage("La configuración vehicular no puede exceder los 30 caracteres.");
            RuleFor(x => x.PlanillaCode).MaximumLength(10).WithMessage("La planilla no puede exceder los 10 caracteres.");
            RuleFor(x => x.Observations).MaximumLength(500).WithMessage("Las observaciones no pueden exceder los 500 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private void AddMeasureRule(Expression<Func<UpdateVehiculoCommand, decimal>> selector, string label)
        {
            RuleFor(selector)
                .InclusiveBetween(0m, MaxMeasure)
                .WithMessage($"{label} debe estar entre 0 y 9999999999.99.");
        }

        private async Task<bool> BeUniquePlate(UpdateVehiculoCommand command, string plate, CancellationToken ct)
        {
            if (!TipoVehiculoParser.TryParse(command.VehicleType, out var vehicleType))
            {
                return true; // el tipo inválido ya lo reporta su propia regla
            }

            var transportistaCode = command.TransportistaCode.Trim().ToUpper();
            var code = command.Code.Trim().ToUpper();
            var licensePlate = plate.Trim().ToUpper();

            return !await _uow.Facturacion.Maestros.Vehiculos.Query()
                .AnyAsync(v =>
                    v.TransportistaCode == transportistaCode &&
                    v.LicensePlate == licensePlate &&
                    !(v.VehicleType == vehicleType && v.Code == code), ct);
        }

        private async Task<bool> ChoferExistsForTransportista(UpdateVehiculoCommand command, string? choferCode, CancellationToken ct)
        {
            var transportistaCode = command.TransportistaCode.Trim().ToUpper();
            var code = choferCode!.Trim().ToUpper();

            return await _uow.Facturacion.Maestros.Choferes.Query()
                .AnyAsync(c => c.TransportistaCode == transportistaCode && c.Code == code, ct);
        }
    }
}
