using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.CreateTrabajadorContable
{
    public class CreateTrabajadorContableCommandValidator
        : AbstractValidator<CreateTrabajadorContableCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorContableCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El trabajador es obligatorio.")
                .Length(5)
                .WithMessage("El código de trabajador debe tener 5 caracteres.")
                .MustAsync(TrabajadorExists)
                .WithMessage("El trabajador indicado no existe.");

            RuleFor(x => x.NumeroItem)
                .InclusiveBetween(1, 10)
                .WithMessage("NumeroItem debe estar entre 1 y 10.")
                .MustAsync(NumeroItemDoesNotExist)
                .WithMessage("El número de ítem ya está registrado para este trabajador.");

            RuleFor(x => x.CuentaContable)
                .MaximumLength(50)
                .WithMessage("La cuenta contable no puede superar los 50 caracteres.");

            RuleFor(x => x.Tipo)
                .MaximumLength(50)
                .WithMessage("El tipo no puede superar los 50 caracteres.");

            RuleFor(x => x.Porcentaje)
                .InclusiveBetween(0, 100)
                .WithMessage("Porcentaje debe estar entre 0 y 100.");
        }

        private async Task<bool> TrabajadorExists(
            string trabajadorCode,
            CancellationToken cancellationToken)
        {
            return await _uow.Rrhh.Maestros.Trabajadores
                .Query()
                .AnyAsync(
                    x => x.Code == trabajadorCode.Trim(),
                    cancellationToken);
        }

        private async Task<bool> NumeroItemDoesNotExist(
            CreateTrabajadorContableCommand command,
            int numeroItem,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorContables
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == command.TrabajadorCode.Trim()
                         && x.NumeroItem == numeroItem,
                    cancellationToken);
        }
    }
}
