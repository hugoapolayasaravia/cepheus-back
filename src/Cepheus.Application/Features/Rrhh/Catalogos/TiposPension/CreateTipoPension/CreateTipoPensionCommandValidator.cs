using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.CreateTipoPension
{
    public class CreateTipoPensionCommandValidator : AbstractValidator<CreateTipoPensionCommand>
    {
        public CreateTipoPensionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de pensión es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}