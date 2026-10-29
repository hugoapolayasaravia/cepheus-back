using FluentValidation;

namespace Cepheus.Application.Features.Administracion.Users.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.")
                .MaximumLength(256).WithMessage("El correo no puede superar los 256 caracteres.");
        }
    }
}
