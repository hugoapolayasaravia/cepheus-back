using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.Refresh
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthenticateResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtTokenService _jwtTokenService;

        public RefreshTokenCommandHandler(IUnitOfWork uow, IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthenticateResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _uow.Administracion.RefreshTokens.Query()
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (storedToken is null || !storedToken.IsActive)
            {
                throw new UnauthorizedAccessException("El refresh token es inválido o expiró. Inicie sesión nuevamente.");
            }

            if (!storedToken.User.IsActive)
            {
                throw new UnauthorizedAccessException("El usuario se encuentra inactivo.");
            }

            // Rotación: se revoca el token usado y se emite uno nuevo. Si alguien
            // reutiliza un refresh token ya revocado, IsActive va a ser false y
            // caerá en el 401 de arriba en el siguiente intento.
            storedToken.RevokedAt = DateTime.UtcNow;

            var roleNames = storedToken.User.UserRoles.Select(ur => ur.Role.Name).ToList();

            var newAccessToken = _jwtTokenService.GenerateAccessToken(storedToken.User, roleNames);
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            var newRefreshToken = new Domain.Administracion.RefreshToken
            {
                Token = newRefreshTokenValue,
                UserId = storedToken.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtTokenService.RefreshTokenExpirationDays)
            };

            await _uow.Administracion.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new AuthenticateResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.AccessTokenExpirationMinutes),
                User = new UserResponse
                {
                    Id = storedToken.User.Id,
                    Email = storedToken.User.Email,
                    FirstName = storedToken.User.FirstName,
                    LastName = storedToken.User.LastName,
                    IsActive = storedToken.User.IsActive,
                    CreatedAt = storedToken.User.CreatedAt,
                    UpdatedAt = storedToken.User.UpdatedAt,
                    RowVersion = storedToken.User.RowVersion,
                    Roles = roleNames
                }
            };
        }
    }

}
