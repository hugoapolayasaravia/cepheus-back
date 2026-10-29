using System.Security.Cryptography;
using System.Text;

namespace Cepheus.Application.Features.Administracion.Users.Common
{
    /// <summary>
    /// Genera el token de restablecimiento y calcula su huella.
    /// En la base solo se guarda la huella; el token en claro viaja únicamente en el enlace del correo.
    /// </summary>
    public static class PasswordResetTokenHelper
    {
        /// <summary>Token aleatorio de 32 bytes, en Base64 apto para URL.</summary>
        public static string GenerateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        /// <summary>SHA-256 del token en hexadecimal minúscula (64 caracteres).</summary>
        public static string Hash(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
