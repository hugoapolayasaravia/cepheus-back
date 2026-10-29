using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.CreateCuentaContable
{
    public class CreateCuentaContableCommandValidator : AbstractValidator<CreateCuentaContableCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCuentaContableCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .MaximumLength(8).WithMessage("El código no puede exceder los 8 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una cuenta contable con ese código.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(70).WithMessage("La descripción no puede exceder los 70 caracteres.");

            RuleFor(x => x.TipoCuentaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de cuenta es obligatorio.")
                .MustAsync(TipoCuentaExists).WithMessage("El tipo de cuenta indicado no existe.");

            RuleFor(x => x.TipoAnexoCode)
                .MustAsync(async (code, ct) =>
                    string.IsNullOrWhiteSpace(code) ||
                    await TipoAnexoExists(code, ct))
                .WithMessage("El tipo de anexo indicado no existe.");

            RuleFor(x => x.LinkAccount1Code)
                .MaximumLength(8).WithMessage("La cuenta de enlace 1 no puede exceder los 8 caracteres.");

            RuleFor(x => x.LinkAccount2Code)
                .MaximumLength(8).WithMessage("La cuenta de enlace 2 no puede exceder los 8 caracteres.");

            RuleFor(x => x.ClosingAccountCode)
                .MaximumLength(8).WithMessage("La cuenta de cierre no puede exceder los 8 caracteres.");

            RuleFor(x => x.CurrencyCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MustAsync(CurrencyExists).WithMessage("La moneda indicada no existe.");

            RuleFor(x => x.AcmFlag)
                .MaximumLength(1)
                .WithMessage("El flag ACM no puede exceder 1 carácter.");

            RuleFor(x => x.PilotFlag)
                .Must(v => string.IsNullOrWhiteSpace(v) ||
                           v.Trim().ToUpper() is "S" or "N")
                .WithMessage("El flag piloto debe ser 'S' o 'N'.");

            RuleFor(x => x.GainsLossesFlag)
                .MaximumLength(1)
                .WithMessage("El flag de ganancias y pérdidas no puede exceder 1 carácter.");
        }

        private async Task<bool> TipoCuentaExists(string code, CancellationToken ct)
            => await _uow.Contabilidad.Catalogos.TiposCuentaContable.Query().AnyAsync(t => t.Code == code.Trim(), ct);

        private async Task<bool> TipoAnexoExists(string code, CancellationToken ct)
            => await _uow.Contabilidad.Catalogos.TiposAnexo.Query().AnyAsync(t => t.Code == code.Trim(), ct);

        private async Task<bool> CurrencyExists(string code, CancellationToken ct)
            => await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> BeUniqueCode(string code, CancellationToken ct)
            => !await _uow.Contabilidad.Maestros.CuentasContables.Query().AnyAsync(c => c.Code == code.Trim(), ct);
    }
}
