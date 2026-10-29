using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.CreateNivelTrabajador
{
    public class CreateNivelTrabajadorCommandValidator : AbstractValidator<CreateNivelTrabajadorCommand>
    {
        public CreateNivelTrabajadorCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del nivel es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}