using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.CreateRegimenPensionario
{
    public class CreateRegimenPensionarioCommandValidator : AbstractValidator<CreateRegimenPensionarioCommand>
    {
        public CreateRegimenPensionarioCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del régimen pensionario es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}