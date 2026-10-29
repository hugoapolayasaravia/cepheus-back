using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.CreateAfp
{
    public class CreateAfpCommandValidator : AbstractValidator<CreateAfpCommand>
    {
        public CreateAfpCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la AFP es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}