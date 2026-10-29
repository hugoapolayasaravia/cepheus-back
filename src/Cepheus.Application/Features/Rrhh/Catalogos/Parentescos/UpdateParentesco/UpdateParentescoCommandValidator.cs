using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.UpdateParentesco
{
    public class UpdateParentescoCommandValidator : AbstractValidator<UpdateParentescoCommand>
    {
        public UpdateParentescoCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del parentesco es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del parentesco es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}