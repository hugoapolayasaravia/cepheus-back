using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.UpdateControlCierre
{
    public class UpdateControlCierreCommandValidator : AbstractValidator<UpdateControlCierreCommand>
    {
        public UpdateControlCierreCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");

            RuleFor(x => x.PeriodCode)
                .NotEmpty().WithMessage("El período es obligatorio.")
                .Matches(@"^\d{4}(0[1-9]|1[0-2])$").WithMessage("El período debe tener formato AAAAMM (ej. 202401).");

            RuleFor(x => x.ClosureDate).NotEmpty().WithMessage("La fecha de cierre es obligatoria.");
        }
    }
}
