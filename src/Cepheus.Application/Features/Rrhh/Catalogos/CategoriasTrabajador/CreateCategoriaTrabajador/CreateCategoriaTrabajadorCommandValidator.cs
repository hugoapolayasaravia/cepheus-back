using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.CreateCategoriaTrabajador
{
    public class CreateCategoriaTrabajadorCommandValidator : AbstractValidator<CreateCategoriaTrabajadorCommand>
    {
        public CreateCategoriaTrabajadorCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la categoría de trabajador es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}