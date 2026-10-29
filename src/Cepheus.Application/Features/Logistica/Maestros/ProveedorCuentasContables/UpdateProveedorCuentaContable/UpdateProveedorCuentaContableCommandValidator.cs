using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.UpdateProveedorCuentaContable
{
    public class UpdateProveedorCuentaContableCommandValidator
        : ProveedorCuentaContableValidator<UpdateProveedorCuentaContableCommand>
    {
        public UpdateProveedorCuentaContableCommandValidator(IUnitOfWork uow) : base(uow)
        {
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
