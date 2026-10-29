using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.CreateControlCierre
{
    public class CreateControlCierreCommandValidator : AbstractValidator<CreateControlCierreCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateControlCierreCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x.PeriodCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El período es obligatorio.")
                .Matches(@"^\d{4}(0[1-9]|1[0-2])$").WithMessage("El período debe tener formato AAAAMM (ej. 202401).");

            RuleFor(x => x)
                .MustAsync(BeUniquePeriod)
                .WithMessage("Ya existe un cierre para esta planta en este período.")
                .OverridePropertyName(nameof(CreateControlCierreCommand.PeriodCode));

            RuleFor(x => x.ClosureDate)
                .NotEmpty().WithMessage("La fecha de cierre es obligatoria.");
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniquePeriod(CreateControlCierreCommand command, CancellationToken ct)
            => !await _uow.Logistica.Maestros.ControlCierres.Query()
                .AnyAsync(c =>
                    c.PlantaCode == command.PlantaCode.Trim().ToUpper() &&
                    c.PeriodCode == command.PeriodCode.Trim(), ct);
    }
}
