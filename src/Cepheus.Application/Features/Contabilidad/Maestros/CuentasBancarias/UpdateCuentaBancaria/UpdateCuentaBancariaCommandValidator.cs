using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.UpdateCuentaBancaria
{
    public class UpdateCuentaBancariaCommandValidator : AbstractValidator<UpdateCuentaBancariaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCuentaBancariaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.BankCode)
                .NotEmpty().WithMessage("El banco es obligatorio.")
                .Length(3).WithMessage("El código de banco debe tener 3 caracteres.");

            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("El número de cuenta es obligatorio.")
                .MaximumLength(20).WithMessage("El número de cuenta no puede exceder los 20 caracteres.");

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
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> CurrencyExists(string code, CancellationToken ct)
            => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> AccountExists(string code, CancellationToken ct)
            => await _uow.Contabilidad.Maestros.CuentasContables.Query().AnyAsync(c => c.Code == code.Trim(), ct);
    }
}
