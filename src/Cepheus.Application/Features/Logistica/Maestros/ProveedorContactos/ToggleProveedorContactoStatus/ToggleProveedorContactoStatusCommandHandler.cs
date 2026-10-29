using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.ToggleProveedorContactoStatus
{
    public class ToggleProveedorContactoStatusCommandHandler : IRequestHandler<ToggleProveedorContactoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleProveedorContactoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleProveedorContactoStatusCommand request, CancellationToken cancellationToken)
        {
            var contacto = await _uow.Logistica.Maestros.ProveedorContactos.GetByIdAsync(request.Id, cancellationToken);

            if (contacto is null)
            {
                throw new KeyNotFoundException($"Contacto {request.Id} no encontrado.");
            }

            contacto.IsActive = !contacto.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return contacto.IsActive;
        }
    }
}
