using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.UpdateProveedorContacto
{
    public class UpdateProveedorContactoCommandValidator : AbstractValidator<UpdateProveedorContactoCommand>
    {
        public UpdateProveedorContactoCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre del contacto es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

            RuleFor(x => x.Position)
                .MaximumLength(100).WithMessage("El cargo no puede exceder los 100 caracteres.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("El teléfono no puede exceder los 30 caracteres.");

            RuleFor(x => x.MobilePhone)
                .MaximumLength(30).WithMessage("El teléfono móvil no puede exceder los 30 caracteres.");

            RuleFor(x => x.Email)
                .MaximumLength(150).WithMessage("El correo no puede exceder los 150 caracteres.")
                .EmailAddress().WithMessage("El correo no tiene un formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}
