using System.Net;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cepheus.Infrastructure.Services
{
    public class UserInvitationService : IUserInvitationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordResetSettings _settings;
        private readonly ILogger<UserInvitationService> _logger;

        public UserInvitationService(
            IUnitOfWork uow,
            IEmailSender emailSender,
            IPasswordResetSettings settings,
            ILogger<UserInvitationService> logger)
        {
            _uow = uow;
            _emailSender = emailSender;
            _settings = settings;
            _logger = logger;
        }

        public async Task<bool> SendAsync(User user, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Solo el enlace más reciente sirve: se invalidan los que sigan pendientes.
            var pendientes = await _uow.Administracion.PasswordResetTokens.Query()
                .Where(t => t.UserId == user.Id && t.UsedAt == null && t.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            foreach (var pendiente in pendientes)
            {
                pendiente.UsedAt = now;
            }

            var token = PasswordResetTokenHelper.GenerateToken();

            await _uow.Administracion.PasswordResetTokens.AddAsync(new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = PasswordResetTokenHelper.Hash(token),
                CreatedAt = now,
                ExpiresAt = now.AddHours(_settings.InvitationExpirationHours)
            }, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            // Mismo enlace y misma pantalla que la recuperación de contraseña; "invitacion=1"
            // solo cambia los textos que ve la persona.
            var enlace = $"{_settings.FrontendBaseUrl.TrimEnd('/')}/restablecer-contrasena?token={Uri.EscapeDataString(token)}&invitacion=1";

            try
            {
                await _emailSender.SendAsync(
                    user.Email,
                    "Bienvenido a Cepheus: crea tu contraseña",
                    ConstruirCuerpo(user.FirstName, user.Email, enlace, _settings.InvitationExpirationHours),
                    cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo enviar la invitación al usuario {UserId}.", user.Id);
                return false;
            }
        }

        private static string ConstruirCuerpo(string nombre, string correo, string enlace, int horas)
        {
            var nombreSeguro = WebUtility.HtmlEncode(nombre);
            var correoSeguro = WebUtility.HtmlEncode(correo);
            var enlaceSeguro = WebUtility.HtmlEncode(enlace);

            return $@"<div style=""font-family:Arial,sans-serif;font-size:14px;color:#222"">
<p>Hola {nombreSeguro},</p>
<p>Se creó una cuenta para ti en Cepheus con el correo {correoSeguro}. Para empezar a usarla, crea tu contraseña con el botón de abajo.</p>
<p><a href=""{enlaceSeguro}"" style=""display:inline-block;padding:10px 18px;background:#1b84ff;color:#fff;text-decoration:none;border-radius:6px"">Crear mi contraseña</a></p>
<p>El enlace vence en {horas} horas y solo puede usarse una vez. Si vence, pide que te envíen otra invitación.</p>
<p>Si no esperabas este correo, puedes ignorarlo.</p>
</div>";
        }
    }
}
