using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria
{
    public class CreateCuentaBancariaCommandValidator : AbstractValidator<CreateCuentaBancariaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCuentaBancariaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.BankCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El banco es obligatorio.")
                .Length(3).WithMessage("El código de banco debe tener 3 caracteres.")
                .MustAsync(BankExists).WithMessage("El banco indicado no existe.");

            RuleFor(x => x.AccountNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de cuenta es obligatorio.")
                .MaximumLength(20).WithMessage("El número de cuenta no puede exceder los 20 caracteres.");

            RuleFor(x => x)
                .MustAsync(BeUniqueAccount).WithMessage("Ya existe esa cuenta bancaria para el banco indicado.")
                .OverridePropertyName(nameof(CreateCuentaBancariaCommand.AccountNumber))
                .When(x => !string.IsNullOrWhiteSpace(x.BankCode) && !string.IsNullOrWhiteSpace(x.AccountNumber));

            RuleFor(x => x.CurrencyCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(CurrencyExists).WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.CheckStart)
                .MaximumLength(10).WithMessage("El cheque inicial no puede exceder los 10 caracteres.");

            RuleFor(x => x.CheckEnd)
                .MaximumLength(10).WithMessage("El cheque final no puede exceder los 10 caracteres.");

            RuleFor(x => x.CheckCurrent)
                .MaximumLength(10).WithMessage("El cheque actual no puede exceder los 10 caracteres.");

            RuleFor(x => x.ControlFlag)
                .NotEmpty().WithMessage("El flag de control es obligatorio.")
                .Length(1).WithMessage("El flag de control debe tener 1 carácter.");

            RuleFor(x => x.AccountCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La cuenta contable es obligatoria.")
                .MaximumLength(8).WithMessage("La cuenta contable no puede exceder los 8 caracteres.")
                .MustAsync(AccountExists).WithMessage("La cuenta contable indicada no existe.");
        }

        private async Task<bool> CurrencyExists(string code, CancellationToken ct)
            => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> AccountExists(string code, CancellationToken ct)
            => await _uow.Contabilidad.Maestros.CuentasContables.Query().AnyAsync(c => c.Code == code.Trim(), ct);

        private async Task<bool> BankExists(string code, CancellationToken ct)
            => await _uow.Comunes.Bancos.Query().AnyAsync(b => b.Code == code.Trim(), ct);

        private async Task<bool> BeUniqueAccount(CreateCuentaBancariaCommand c, CancellationToken ct)
        {
            var bank = c.BankCode.Trim();
            var number = c.AccountNumber.Trim();

            return !await _uow.Contabilidad.Maestros.CuentasBancarias.Query()
                .AnyAsync(x => x.BankCode == bank && x.AccountNumber == number, ct);
        }
    }
}
