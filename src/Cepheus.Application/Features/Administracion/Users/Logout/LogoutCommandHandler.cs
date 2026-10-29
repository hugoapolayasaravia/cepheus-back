using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus.Application.Features.Administracion.Users.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IUnitOfWork _uow;

        public LogoutCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _uow.Administracion.RefreshTokens.Query()
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            // Idempotente a propósito: si el token no existe o ya estaba revocado,
            // el resultado deseado (que ese token no sirva más) ya se cumple.
            // No se lanza error para no filtrar si un token existe o no (mismo
            // criterio que en Authenticate: mensajes genéricos, sin enumeración).
            if (storedToken is null || storedToken.RevokedAt is not null)
            {
                return;
            }

            storedToken.RevokedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }

}
