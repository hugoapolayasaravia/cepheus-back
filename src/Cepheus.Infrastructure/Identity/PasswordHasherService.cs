using Cepheus.Application.Comun.Interfaces;

namespace Cepheus.Infrastructure.Identity
{
    public class PasswordHasherService : IPasswordHasher
    {
        // Work factor 12: balance estándar entre seguridad y performance para 2025+.
        private const int WorkFactor = 12;

        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);

        public bool Verify(string password, string passwordHash)
            => BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

}
