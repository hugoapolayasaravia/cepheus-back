using Cepheus.Application.Comun.Interfaces;

namespace Cepheus.Infrastructure.Services
{
    /// <summary>
    /// Se bindea desde la sección "PasswordReset" de appsettings (ver InfrastructureServiceRegistration).
    /// </summary>
    public class PasswordResetSettings : IPasswordResetSettings
    {
        public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
        public int TokenExpirationMinutes { get; set; } = 30;
        public int InvitationExpirationHours { get; set; } = 72;
    }
}
