namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Ajustes de la recuperación de contraseña y de las invitaciones. La implementación vive en
    /// Infrastructure y se llena desde la sección "PasswordReset" de appsettings.
    /// </summary>
    public interface IPasswordResetSettings
    {
        /// <summary>Dirección base del frontend (sin barra final), por ejemplo http://localhost:4200.</summary>
        string FrontendBaseUrl { get; }

        /// <summary>Minutos de vigencia del enlace de recuperación de contraseña.</summary>
        int TokenExpirationMinutes { get; }

        /// <summary>Horas de vigencia del enlace de invitación de un usuario nuevo.</summary>
        int InvitationExpirationHours { get; }
    }
}
