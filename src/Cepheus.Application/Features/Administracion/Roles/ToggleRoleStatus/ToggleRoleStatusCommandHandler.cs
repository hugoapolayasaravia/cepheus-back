using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.ToggleRoleStatus
{
    public class ToggleRoleStatusCommandHandler : IRequestHandler<ToggleRoleStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleRoleStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleRoleStatusCommand request, CancellationToken cancellationToken)
        {
            var role = await _uow.Administracion.Roles.GetByIdAsync(request.Id, cancellationToken);

            if (role is null)
            {
                throw new KeyNotFoundException($"Rol {request.Id} no encontrado.");
            }

            // Entidad ya trackeada (GetByIdAsync -> FindAsync), no hace falta llamar Update().
            role.IsActive = !role.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return role.IsActive;
        }
    }

}
