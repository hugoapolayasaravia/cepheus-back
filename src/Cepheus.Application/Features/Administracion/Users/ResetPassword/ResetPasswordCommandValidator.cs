using FluentValidation;

namespace Cepheus.Application.Features.Administracion.Users.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("El enlace no es válido o ha expirado. Solicita uno nuevo.")
                .MaximumLength(200).WithMessage("El enlace no es válido o ha expirado. Solicita uno nuevo.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La contraseña nueva es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña nueva debe tener al menos 8 caracteres.");
        }
    }
}
