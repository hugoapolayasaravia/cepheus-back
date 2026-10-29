using FluentValidation;

namespace Cepheus.Application.Features.Administracion.Users.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña actual es obligatoria.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La contraseña nueva es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña nueva debe tener al menos 8 caracteres.")
                .NotEqual(x => x.CurrentPassword).WithMessage("La contraseña nueva debe ser distinta de la actual.");
        }
    }

}
