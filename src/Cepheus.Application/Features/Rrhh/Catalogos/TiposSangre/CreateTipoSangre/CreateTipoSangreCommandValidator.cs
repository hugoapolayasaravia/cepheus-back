using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.CreateTipoSangre
{
    public class CreateTipoSangreCommandValidator : AbstractValidator<CreateTipoSangreCommand>
    {
        public CreateTipoSangreCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de sangre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");
        }
    }
}