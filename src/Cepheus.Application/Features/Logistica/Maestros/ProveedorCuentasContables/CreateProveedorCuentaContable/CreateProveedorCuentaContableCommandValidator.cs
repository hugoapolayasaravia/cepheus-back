using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.CreateProveedorCuentaContable
{
    public class CreateProveedorCuentaContableCommandValidator
        : ProveedorCuentaContableValidator<CreateProveedorCuentaContableCommand>
    {
        public CreateProveedorCuentaContableCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x)
                .MustAsync(async (x, ct) =>
                {
                    var prv = x.ProveedorCode.Trim().ToUpper();
                    var mon = x.MonedaCode.Trim().ToUpper();
                    return !await uow.Logistica.Maestros.ProveedorCuentasContables.Query()
                        .AnyAsync(c => c.ProveedorCode == prv && c.MonedaCode == mon, ct);
                })
                .WithMessage("El proveedor ya tiene una cuenta por pagar para esa moneda.")
                .OverridePropertyName("MonedaCode")
                .When(x => !string.IsNullOrWhiteSpace(x.ProveedorCode) && !string.IsNullOrWhiteSpace(x.MonedaCode));
        }
    }
}
