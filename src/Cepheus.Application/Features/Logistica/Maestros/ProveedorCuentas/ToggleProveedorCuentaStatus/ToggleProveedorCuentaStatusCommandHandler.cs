using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.ToggleProveedorCuentaStatus
{
    public class ToggleProveedorCuentaStatusCommandHandler : IRequestHandler<ToggleProveedorCuentaStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleProveedorCuentaStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleProveedorCuentaStatusCommand request, CancellationToken cancellationToken)
        {
            var cuenta = await _uow.Logistica.Maestros.ProveedorCuentas.GetByIdAsync(request.Id, cancellationToken);

            if (cuenta is null)
            {
                throw new KeyNotFoundException($"Cuenta {request.Id} no encontrada.");
            }

            cuenta.IsActive = !cuenta.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return cuenta.IsActive;
        }
    }
}
