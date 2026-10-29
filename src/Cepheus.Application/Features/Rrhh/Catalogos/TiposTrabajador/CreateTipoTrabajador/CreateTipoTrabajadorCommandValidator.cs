using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.CreateTipoTrabajador
{
    public class CreateTipoTrabajadorCommandValidator : AbstractValidator<CreateTipoTrabajadorCommand>
    {
        public CreateTipoTrabajadorCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de trabajador es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}