using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var hash = PasswordResetTokenHelper.Hash(request.Token.Trim());
            var now = DateTime.UtcNow;

            var resetToken = await _uow.Administracion.PasswordResetTokens.Query()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

            // Mismo mensaje para token inexistente, vencido o ya usado: no se da pistas.
            if (resetToken is null || !resetToken.IsActive || !resetToken.User.IsActive)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Token", "El enlace no es válido o ha expirado. Solicita uno nuevo.")
                });
            }

            // Entidades ya trackeadas por la consulta; no hace falta llamar Update().
            resetToken.User.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            resetToken.UsedAt = now;

            // Cualquier otro enlace pendiente del usuario queda inutilizado.
            var otrosEnlaces = await _uow.Administracion.PasswordResetTokens.Query()
                .Where(t => t.UserId == resetToken.UserId && t.Id != resetToken.Id && t.UsedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var otro in otrosEnlaces)
            {
                otro.UsedAt = now;
            }

            // Se cierran las sesiones abiertas: sus refresh tokens dejan de servir.
            var sesiones = await _uow.Administracion.RefreshTokens.Query()
                .Where(rt => rt.UserId == resetToken.UserId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var sesion in sesiones)
            {
                sesion.RevokedAt = now;
            }

            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
