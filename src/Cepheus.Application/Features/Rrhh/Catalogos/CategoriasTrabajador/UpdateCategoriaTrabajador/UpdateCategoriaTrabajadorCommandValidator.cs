using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.UpdateCategoriaTrabajador
{
    public class UpdateCategoriaTrabajadorCommandValidator : AbstractValidator<UpdateCategoriaTrabajadorCommand>
    {
        public UpdateCategoriaTrabajadorCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la categoría de trabajador es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la categoría de trabajador es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}