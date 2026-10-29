using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.CreateProveedorContacto
{
    public class CreateProveedorContactoCommandValidator : AbstractValidator<CreateProveedorContactoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorContactoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El proveedor es obligatorio.")
                .Length(5).WithMessage("El código de proveedor debe tener 5 caracteres.")
                .MustAsync(ProveedorExists).WithMessage("El proveedor indicado no existe.");

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

        private async Task<bool> ProveedorExists(string proveedorCode, CancellationToken cancellationToken)
            => await _uow.Logistica.Maestros.Proveedores.Query()
                .AnyAsync(p => p.Code == proveedorCode.Trim().ToUpper(), cancellationToken);
    }
}
