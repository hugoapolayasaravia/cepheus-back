using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.CreateTipoContrato
{
    public class CreateTipoContratoCommandValidator : AbstractValidator<CreateTipoContratoCommand>
    {
        public CreateTipoContratoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de contrato es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}