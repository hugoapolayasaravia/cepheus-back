using Cepheus.Domain.Administracion;

namespace Cepheus.Application.Comun.Interfaces
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Genera el Access Token JWT. Claims mínimos a propósito: sub (UserId),
        /// username, email y nombres de Role (NO permisos — esos se piden aparte
        /// vía el endpoint /me para no sobrecargar el token).
        /// </summary>
        string GenerateAccessToken(User user, IEnumerable<string> roles);

        /// <summary>
        /// Genera un token aleatorio criptográficamente seguro para el Refresh Token.
        /// No es JWT, es un valor opaco que se guarda en la tabla RefreshTokens.
        /// </summary>
        string GenerateRefreshToken();

        int AccessTokenExpirationMinutes { get; }
        int RefreshTokenExpirationDays { get; }
    }

}
