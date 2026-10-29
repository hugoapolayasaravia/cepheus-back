using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.CreateTrabajadorCuentaBancaria
{
    public class CreateTrabajadorCuentaBancariaCommandValidator
        : AbstractValidator<CreateTrabajadorCuentaBancariaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorCuentaBancariaCommandValidator(IUnitOfWork uow)
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

            RuleFor(x => x.TipoCuentaCode)
                .MustAsync(TipoCuentaExists)
                .WithMessage("El tipo de cuenta indicado no existe.");

            RuleFor(x => x.BancoCode)
                .MustAsync(BancoExists)
                .WithMessage("El banco indicado no existe.");

            RuleFor(x => x.MonedaCode)
                .MustAsync(MonedaExists)
                .WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.NumeroCuenta)
                .MaximumLength(50)
                .WithMessage("El número de cuenta no puede superar los 50 caracteres.");

            RuleFor(x => x.TipoOperacion)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("TipoOperacion es obligatorio.")
                .MaximumLength(50)
                .WithMessage("TipoOperacion no puede superar los 50 caracteres.");

            RuleFor(x => x)
                .MustAsync(NoExisteOtraCuentaPrincipal)
                .When(x => x.Principal)
                .WithMessage("El trabajador ya tiene una cuenta bancaria principal.");
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

        private async Task<bool> TipoCuentaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Rrhh.Catalogos.TiposCuenta
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> BancoExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Comunes.Bancos
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> MonedaExists(
            string? code,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            return await _uow.Comunes.Monedas
                .Query()
                .AnyAsync(
                    x => x.Code == code.Trim(),
                    cancellationToken);
        }

        private async Task<bool> NoExisteOtraCuentaPrincipal(
            CreateTrabajadorCuentaBancariaCommand command,
            CancellationToken cancellationToken)
        {
            return !await _uow.Rrhh.Maestros.TrabajadorCuentaBancarias
                .Query()
                .AnyAsync(
                    x => x.TrabajadorCode == command.TrabajadorCode.Trim()
                         && x.Principal
                         && x.IsActive,
                    cancellationToken);
        }
    }
}
