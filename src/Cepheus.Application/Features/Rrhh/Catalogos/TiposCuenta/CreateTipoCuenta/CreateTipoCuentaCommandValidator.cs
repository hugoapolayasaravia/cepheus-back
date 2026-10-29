using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.CreateTipoCuenta
{
    public class CreateTipoCuentaCommandValidator : AbstractValidator<CreateTipoCuentaCommand>
    {
        public CreateTipoCuentaCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de cuenta es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        }
    }
}