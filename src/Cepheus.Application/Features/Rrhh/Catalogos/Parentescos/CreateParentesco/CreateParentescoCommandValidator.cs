using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.CreateParentesco
{
    public class CreateParentescoCommandValidator : AbstractValidator<CreateParentescoCommand>
    {
        public CreateParentescoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del parentesco es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}