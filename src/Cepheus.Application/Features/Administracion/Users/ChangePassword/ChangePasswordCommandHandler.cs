using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(
            IUnitOfWork uow,
            IPasswordHasher passwordHasher,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.Id != _currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("No puede cambiar la contraseña de otro usuario.");
            }

            var user = await _uow.Administracion.Users.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
            {
                throw new KeyNotFoundException($"Usuario {request.Id} no encontrado.");
            }

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            {
                // 400 con el campo CurrentPassword. Antes era un 401, que el cliente interpreta como
                // "sesión vencida" (intenta renovar el token) y no como un dato mal escrito.
                throw new ValidationException(new[]
                {
                    new ValidationFailure("CurrentPassword", "La contraseña actual no es correcta.")
                });
            }

            // Entidad ya trackeada (GetByIdAsync -> FindAsync); no hace falta llamar Update().
            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

            // Al cambiar la contraseña se cierran todas las sesiones abiertas (también las de otros
            // dispositivos): sus refresh tokens dejan de servir. Quien cambió la contraseña vuelve a entrar.
            var ahora = DateTime.UtcNow;
            var sesiones = await _uow.Administracion.RefreshTokens.Query()
                .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var sesion in sesiones)
            {
                sesion.RevokedAt = ahora;
            }

            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
