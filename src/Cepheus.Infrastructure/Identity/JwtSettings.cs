namespace Cepheus.Infrastructure.Identity
{
    /// <summary>
    /// Se bindea desde la sección "Jwt" de appsettings.json (ver InfrastructureServiceRegistration).
    /// </summary>
    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenExpirationMinutes { get; set; } = 30;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }

}
