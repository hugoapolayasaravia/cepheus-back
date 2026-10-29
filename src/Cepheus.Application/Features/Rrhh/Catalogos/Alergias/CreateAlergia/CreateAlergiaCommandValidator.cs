using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.CreateAlergia
{
    public class CreateAlergiaCommandValidator : AbstractValidator<CreateAlergiaCommand>
    {
        public CreateAlergiaCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la alergia es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}