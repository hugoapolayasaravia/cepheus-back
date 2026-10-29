using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.CreateTipoSctr
{
    public class CreateTipoSctrCommandValidator : AbstractValidator<CreateTipoSctrCommand>
    {
        public CreateTipoSctrCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de SCTR es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}