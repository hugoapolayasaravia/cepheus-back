using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.CreateArticuloUbicaciones
{
    public class CreateArticuloUbicacionesCommandValidator : AbstractValidator<CreateArticuloUbicacionesCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloUbicacionesCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la ubicación es obligatorio.")
                .MaximumLength(80).WithMessage("El nombre no puede superar los 80 caracteres.")
                .MustAsync(BeUniqueNamePerPlanta).WithMessage("Ya existe una ubicación con ese nombre registrada en la misma planta.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.Zone)
                .MaximumLength(40).WithMessage("La zona no puede superar los 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Zone));

            RuleFor(x => x.Rack)
                .MaximumLength(20).WithMessage("El rack no puede superar los 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Rack));

            RuleFor(x => x.Shelf)
                .MaximumLength(20).WithMessage("El estante/shelf no puede superar los 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Shelf));

            RuleFor(x => x.Locker)
                .MaximumLength(20).WithMessage("El casillero/locker no puede superar los 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Locker));
        }

        private async Task<bool> BeUniqueNamePerPlanta(CreateArticuloUbicacionesCommand command, string name, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(command.PlantaCode))
                return true;

            var plantaCodeClean = command.PlantaCode.Trim().ToUpper();
            var nameClean = name.Trim().ToUpper();

            return !await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .AnyAsync(u => u.PlantaCode == plantaCodeClean && u.Name.ToUpper() == nameClean, ct);
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query()
                .AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);
    }
}