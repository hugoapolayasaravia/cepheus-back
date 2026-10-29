using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.UpdateProveedorCuentaContable
{
    public class UpdateProveedorCuentaContableCommandHandler
        : IRequestHandler<UpdateProveedorCuentaContableCommand, ProveedorCuentaContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorCuentaContableResponse> Handle(
            UpdateProveedorCuentaContableCommand request, CancellationToken cancellationToken)
        {
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Maestros.ProveedorCuentasContables.Query()
                .FirstOrDefaultAsync(c => c.ProveedorCode == prv && c.MonedaCode == mon, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"El proveedor {prv} no tiene cuenta por pagar para la moneda {mon}.");
            }

            entity.AccountingAccountCode = request.AccountingAccountCode.Trim().ToUpperInvariant();
            entity.RowVersion = request.RowVersion;

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cuenta fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return ProveedorCuentaContableMapper.Map(entity);
        }
    }
}
