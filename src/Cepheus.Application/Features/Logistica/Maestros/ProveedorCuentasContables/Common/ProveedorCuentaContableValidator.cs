using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common
{
    public abstract class ProveedorCuentaContableValidator<T> : AbstractValidator<T>
        where T : IProveedorCuentaContableDatos
    {
        protected ProveedorCuentaContableValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.ProveedorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El proveedor es obligatorio.")
                .MaximumLength(5)
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Maestros.Proveedores.Query()
                        .AnyAsync(p => p.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El proveedor no existe.");

            RuleFor(x => x.MonedaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La moneda es obligatoria.")
                .MaximumLength(3)
                .MustAsync(async (c, ct) =>
                    await uow.Comunes.Monedas.Query()
                        .AnyAsync(m => m.Code == c.Trim().ToUpper(), ct))
                .WithMessage("La moneda no existe.");

            RuleFor(x => x.AccountingAccountCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La cuenta contable es obligatoria.")
                .Must(c => c.Trim().Length == 8 && (c.Trim().StartsWith("42") || c.Trim().StartsWith("46")))
                .WithMessage("La cuenta por pagar debe tener 8 caracteres y empezar con 42 o 46.")
                .MustAsync(async (c, ct) =>
                    await uow.Contabilidad.Maestros.CuentasContables.Query()
                        .AnyAsync(a => a.Code == c.Trim().ToUpper() && a.IsActive, ct))
                .WithMessage("La cuenta contable no existe o está inactiva.");
        }
    }
}
