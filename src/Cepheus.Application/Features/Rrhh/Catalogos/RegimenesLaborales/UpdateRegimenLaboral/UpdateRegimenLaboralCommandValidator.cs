using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.UpdateRegimenLaboral
{
    public class UpdateRegimenLaboralCommandValidator : AbstractValidator<UpdateRegimenLaboralCommand>
    {
        public UpdateRegimenLaboralCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del régimen laboral es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del régimen laboral es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}