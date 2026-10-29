using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.ToggleUserStatus
{
    public class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public ToggleUserStatusCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _uow.Administracion.Users.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
            {
                throw new KeyNotFoundException($"Usuario {request.Id} no encontrado.");
            }

            if (user.Id == _currentUserService.UserId)
            {
                throw new InvalidOperationException("No puede desactivar su propio usuario.");
            }

            // Entidad ya está trackeada (GetByIdAsync -> FindAsync), no hace falta llamar Update().
            user.IsActive = !user.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return user.IsActive;
        }
    }

}
