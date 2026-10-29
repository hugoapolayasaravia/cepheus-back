using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.CreateSubOcupacion
{
    public class CreateSubOcupacionCommandValidator : AbstractValidator<CreateSubOcupacionCommand>
    {
        public CreateSubOcupacionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la sub ocupación es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}