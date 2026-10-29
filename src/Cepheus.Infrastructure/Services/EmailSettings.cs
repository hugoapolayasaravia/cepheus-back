namespace Cepheus.Infrastructure.Services
{
    /// <summary>
    /// Se bindea desde la sección "Email" de appsettings (ver InfrastructureServiceRegistration).
    /// UserName y Password NO deben ir en appsettings.json del repositorio: usar user-secrets
    /// o variables de entorno (Email__UserName, Email__Password).
    /// </summary>
    public class EmailSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Cepheus";

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(Host)
            && !string.IsNullOrWhiteSpace(UserName)
            && !string.IsNullOrWhiteSpace(Password)
            && !string.IsNullOrWhiteSpace(FromAddress);
    }
}
