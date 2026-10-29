namespace Cepheus.Domain.Administracion
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = default!;

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// No mapeado a columna: se calcula en memoria.
        /// Se debe marcar como .Ignore() en RefreshTokenConfiguration.
        /// </summary>
        public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }

}
