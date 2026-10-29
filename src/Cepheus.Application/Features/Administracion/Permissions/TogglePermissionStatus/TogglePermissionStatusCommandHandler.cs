using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.TogglePermissionStatus
{
    public class TogglePermissionStatusCommandHandler : IRequestHandler<TogglePermissionStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public TogglePermissionStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(TogglePermissionStatusCommand request, CancellationToken cancellationToken)
        {
            var permission = await _uow.Administracion.Permissions.GetByIdAsync(request.Id, cancellationToken);

            if (permission is null)
            {
                throw new KeyNotFoundException($"Permiso {request.Id} no encontrado.");
            }

            permission.IsActive = !permission.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return permission.IsActive;
        }
    }

}
