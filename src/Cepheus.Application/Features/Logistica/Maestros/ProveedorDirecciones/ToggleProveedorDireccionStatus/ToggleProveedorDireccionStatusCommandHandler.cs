using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.ToggleProveedorDireccionStatus
{
    public class ToggleProveedorDireccionStatusCommandHandler : IRequestHandler<ToggleProveedorDireccionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleProveedorDireccionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleProveedorDireccionStatusCommand request, CancellationToken cancellationToken)
        {
            var direccion = await _uow.Logistica.Maestros.ProveedorDirecciones.GetByIdAsync(request.Id, cancellationToken);

            if (direccion is null)
            {
                throw new KeyNotFoundException($"Dirección {request.Id} no encontrada.");
            }

            direccion.IsActive = !direccion.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return direccion.IsActive;
        }
    }
}
