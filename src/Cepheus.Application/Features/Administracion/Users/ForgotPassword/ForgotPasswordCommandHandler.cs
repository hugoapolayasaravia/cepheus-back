using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Cepheus.Application.Features.Administracion.Users.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordResetSettings _settings;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IUnitOfWork uow,
            IEmailSender emailSender,
            IPasswordResetSettings settings,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _uow = uow;
            _emailSender = emailSender;
            _settings = settings;
            _logger = logger;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _uow.Administracion.Users.Query()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            // Si el correo no existe o el usuario está inactivo, se termina en silencio:
            // la respuesta al cliente es idéntica y no revela qué correos están registrados.
            if (user is null || !user.IsActive)
            {
                return;
            }

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
                ExpiresAt = now.AddMinutes(_settings.TokenExpirationMinutes)
            }, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            var enlace = $"{_settings.FrontendBaseUrl.TrimEnd('/')}/restablecer-contrasena?token={Uri.EscapeDataString(token)}";

            try
            {
                await _emailSender.SendAsync(
                    user.Email,
                    "Restablece tu contraseña de Cepheus",
                    ConstruirCuerpo(user.FirstName, enlace, _settings.TokenExpirationMinutes),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                // Un fallo del correo no debe cambiar la respuesta al cliente (revelaría
                // que el correo existe). Se registra para poder revisarlo.
                _logger.LogError(ex, "No se pudo enviar el correo de recuperación al usuario {UserId}.", user.Id);
            }
        }

        private static string ConstruirCuerpo(string nombre, string enlace, int minutos)
        {
            var nombreSeguro = WebUtility.HtmlEncode(nombre);
            var enlaceSeguro = WebUtility.HtmlEncode(enlace);

            return $@"<div style=""font-family:Arial,sans-serif;font-size:14px;color:#222"">
<p>Hola {nombreSeguro},</p>
<p>Recibimos una solicitud para restablecer la contraseña de tu cuenta de Cepheus.</p>
<p><a href=""{enlaceSeguro}"" style=""display:inline-block;padding:10px 18px;background:#1b84ff;color:#fff;text-decoration:none;border-radius:6px"">Restablecer contraseña</a></p>
<p>El enlace vence en {minutos} minutos y solo puede usarse una vez.</p>
<p>Si no fuiste tú, ignora este correo: tu contraseña no cambiará.</p>
</div>";
        }
    }
}
