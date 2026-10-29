using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.DeleteProveedorCuentaContable
{
    public class DeleteProveedorCuentaContableCommandHandler : IRequestHandler<DeleteProveedorCuentaContableCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteProveedorCuentaContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteProveedorCuentaContableCommand request, CancellationToken cancellationToken)
        {
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            var entity = await _uow.Logistica.Maestros.ProveedorCuentasContables.Query()
                .FirstOrDefaultAsync(c => c.ProveedorCode == prv && c.MonedaCode == mon, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"El proveedor {prv} no tiene cuenta por pagar para la moneda {mon}.");
            }

            _uow.Logistica.Maestros.ProveedorCuentasContables.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
