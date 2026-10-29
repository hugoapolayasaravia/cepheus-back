using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUserCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email no tiene un formato válido.")
                .MaximumLength(150)
                .MustAsync(BeUniqueEmail)
                .WithMessage("El email ya está registrado.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(100)
                .MustAsync(BeUniqueFullName)
                .WithMessage("Ya existe un usuario registrado con ese nombre y apellido.");

            RuleFor(x => x.TrabajadorCode)
                .MustAsync(BeValidTrabajadorCode)
                .WithMessage("El TrabajadorCode indicado no existe en RRHH.");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion es obligatorio para validar concurrencia.");
        }

        private async Task<bool> BeValidTrabajadorCode(string? trabajadorCode, CancellationToken cancellationToken)
        {
            // Es opcional
            if (string.IsNullOrWhiteSpace(trabajadorCode))
                return true;

            return await _uow.Rrhh.Maestros.Trabajadores.Query()
                .AnyAsync(t => t.Code == trabajadorCode.Trim(), cancellationToken);
        }

        private async Task<bool> BeUniqueEmail(
            UpdateUserCommand command,
            string email,
            CancellationToken cancellationToken)
            => !await _uow.Administracion.Users.Query()
                .AnyAsync(
                    u => u.Email == email.Trim().ToLower() &&
                         u.Id != command.Id,
                    cancellationToken);

        private async Task<bool> BeUniqueFullName(UpdateUserCommand command, string lastName, CancellationToken cancellationToken)
            => !await _uow.Administracion.Users.Query()
                .AnyAsync(
                    u =>
                        u.FirstName == command.FirstName.Trim() &&
                        u.LastName == lastName.Trim() &&
                        u.Id != command.Id,
                    cancellationToken);
    }
}