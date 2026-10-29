using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.Authenticate
{
    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AuthenticateResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticateCommandHandler(
            IUnitOfWork uow,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthenticateResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _uow.Administracion.Users.Query()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            // Mensaje deliberadamente genérico: no revela si el problema fue
            // el email o la contraseña (evita enumeración de usuarios).
            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email o contraseña incorrectos.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("El usuario se encuentra inactivo.");
            }

            var roleNames = user.UserRoles.Select(ur => ur.Role.Name).ToList();

            var accessToken = _jwtTokenService.GenerateAccessToken(user, roleNames);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtTokenService.RefreshTokenExpirationDays)
            };

            await _uow.Administracion.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new AuthenticateResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenService.AccessTokenExpirationMinutes),
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    RowVersion = user.RowVersion,
                    Roles = roleNames
                }
            };
        }
    }




}
