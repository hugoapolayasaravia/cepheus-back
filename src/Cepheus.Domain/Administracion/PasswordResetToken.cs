namespace Cepheus.Domain.Administracion
{
    /// <summary>
    /// Token de un solo uso para restablecer la contraseña.
    /// Solo se guarda la huella (SHA-256) del token; el token en claro viaja únicamente
    /// en el enlace del correo.
    /// </summary>
    public class PasswordResetToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        /// <summary>SHA-256 del token en hexadecimal (64 caracteres).</summary>
        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Se llena cuando el token se usa o se invalida.</summary>
        public DateTime? UsedAt { get; set; }

        /// <summary>
        /// No mapeado a columna: se calcula en memoria.
        /// Se marca con .Ignore() en PasswordResetTokenConfiguration.
        /// </summary>
        public bool IsActive => UsedAt is null && DateTime.UtcNow < ExpiresAt;

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
