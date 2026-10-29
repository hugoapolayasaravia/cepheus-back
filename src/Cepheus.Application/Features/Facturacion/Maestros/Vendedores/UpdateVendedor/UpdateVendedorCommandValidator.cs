using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.UpdateVendedor
{
    public class UpdateVendedorCommandValidator : AbstractValidator<UpdateVendedorCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateVendedorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(4).WithMessage("El código debe tener 4 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Abbreviation)
                .MaximumLength(10).WithMessage("La abreviatura no puede exceder los 10 caracteres.");

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("La dirección no puede exceder los 200 caracteres.");

            RuleFor(x => x.Phone)
                .MaximumLength(30).WithMessage("El teléfono no puede exceder los 30 caracteres.");

            RuleFor(x => x.Email)
                .MaximumLength(150).WithMessage("El correo no puede exceder los 150 caracteres.")
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("El correo no tiene un formato válido.");

            RuleFor(x => x.Title)
                .MaximumLength(10).WithMessage("El título no puede exceder los 10 caracteres.");

            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .MustAsync(UserExists).WithMessage("El usuario indicado no existe.")
                .MustAsync(UserNotLinked).WithMessage("El usuario ya está asignado a otro registro.")
                .When(x => x.UserId.HasValue);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> UserExists(int? userId, CancellationToken ct)
            => await _uow.Administracion.Users.Query().AnyAsync(u => u.Id == userId!.Value, ct);

        private async Task<bool> UserNotLinked(UpdateVendedorCommand command, int? userId, CancellationToken ct)
            => !await _uow.Facturacion.Maestros.Vendedores.Query().AnyAsync(x => x.UserId == userId!.Value && x.Code != command.Code, ct);
    }
}
