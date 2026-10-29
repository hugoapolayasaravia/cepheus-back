using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.UpdateArticuloUbicaciones
{
    public class UpdateArticuloUbicacionesCommandValidator : AbstractValidator<UpdateArticuloUbicacionesCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloUbicacionesCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de ubicación es obligatorio.")
                .Length(4).WithMessage("El código debe tener exactamente 4 caracteres.");

            RuleFor(x => x.PlantaCode)
                .NotEmpty().WithMessage("El código de planta es obligatorio.")
                .MaximumLength(2).WithMessage("El código de planta no puede exceder 2 caracteres.")
                .MustAsync(PlantaExisteAsync).WithMessage("La planta especificada no existe.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la ubicación es obligatorio.")
                .MaximumLength(80).WithMessage("El nombre de la ubicación no puede exceder 80 caracteres.")
                .MustAsync(NombreUnicoEnPlantaExcluyendoActualAsync)
                .WithMessage("Ya existe otra ubicación con ese nombre en la planta indicada.");

            RuleFor(x => x.Zone)
                .MaximumLength(20).WithMessage("La zona no puede exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Zone));

            RuleFor(x => x.Rack)
                .MaximumLength(20).WithMessage("El rack no puede exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Rack));

            RuleFor(x => x.Shelf)
                .MaximumLength(20).WithMessage("El estante (shelf) no puede exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Shelf));

            RuleFor(x => x.Locker)
                .MaximumLength(20).WithMessage("El casillero (locker) no puede exceder 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Locker));
        }

        private async Task<bool> PlantaExisteAsync(string plantaCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(plantaCode)) return false;
            var codeClean = plantaCode.Trim();

            return await _uow.Comunes.Plantas.Query()
                .AnyAsync(x => x.Code == codeClean, cancellationToken);
        }

        private async Task<bool> NombreUnicoEnPlantaExcluyendoActualAsync(
            UpdateArticuloUbicacionesCommand command,
            string name,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(command.PlantaCode)) return true;

            var nameClean = name.Trim().ToUpper();
            var plantaCodeClean = command.PlantaCode.Trim();
            var codeClean = command.Code.Trim();

            return !await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .AnyAsync(x => x.PlantaCode == plantaCodeClean
                            && x.Name.ToUpper() == nameClean
                            && x.Code != codeClean, cancellationToken);
        }
    }
}