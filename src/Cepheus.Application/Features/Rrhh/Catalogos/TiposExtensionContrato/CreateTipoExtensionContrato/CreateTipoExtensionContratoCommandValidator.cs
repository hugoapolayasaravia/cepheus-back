using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.CreateTipoExtensionContrato
{
    public class CreateTipoExtensionContratoCommandValidator : AbstractValidator<CreateTipoExtensionContratoCommand>
    {
        public CreateTipoExtensionContratoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de extensión de contrato es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}