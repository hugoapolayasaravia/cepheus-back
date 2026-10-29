using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.UpdateProveedorCuenta
{
    public class UpdateProveedorCuentaCommandValidator : AbstractValidator<UpdateProveedorCuentaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorCuentaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.BancoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El banco es obligatorio.")
                .MustAsync(BancoExists).WithMessage("El banco indicado no existe.");

            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("El tipo de cuenta no es válido.");

            RuleFor(x => x.AccountNumber)
                .NotEmpty().WithMessage("El número de cuenta es obligatorio.")
                .MaximumLength(30).WithMessage("El número de cuenta no puede exceder los 30 caracteres.");

            RuleFor(x => x.InterbankCode)
                .MaximumLength(20).WithMessage("El código interbancario no puede exceder los 20 caracteres.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(MonedaExists).WithMessage("La moneda indicada no existe.");
        }

        private async Task<bool> BancoExists(string bancoCode, CancellationToken cancellationToken)
            => await _uow.Comunes.Bancos.Query()
                .AnyAsync(b => b.Code == bancoCode.Trim().ToUpper(), cancellationToken);

        private async Task<bool> MonedaExists(string monedaCode, CancellationToken cancellationToken)
            => await _uow.Comunes.  Monedas.Query()
                .AnyAsync(m => m.Code == monedaCode.Trim().ToUpper(), cancellationToken);
    }
}
