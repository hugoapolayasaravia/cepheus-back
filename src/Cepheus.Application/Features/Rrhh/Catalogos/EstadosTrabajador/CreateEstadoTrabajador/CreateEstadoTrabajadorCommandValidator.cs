using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.CreateEstadoTrabajador
{
    public class CreateEstadoTrabajadorCommandValidator : AbstractValidator<CreateEstadoTrabajadorCommand>
    {
        public CreateEstadoTrabajadorCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del estado de trabajador es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}