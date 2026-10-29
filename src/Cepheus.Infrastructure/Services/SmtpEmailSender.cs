using Cepheus.Application.Comun.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Cepheus.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly IHostEnvironment _environment;

        public SmtpEmailSender(
            IOptions<EmailSettings> settings,
            ILogger<SmtpEmailSender> logger,
            IHostEnvironment environment)
        {
            _settings = settings.Value;
            _logger = logger;
            _environment = environment;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (!_settings.IsConfigured)
            {
                // Sin SMTP configurado: solo en desarrollo se escribe el correo en la consola
                // de la API (para poder probar sin enviar correos reales). En otros entornos
                // se falla, para no esconder un problema de configuración.
                if (_environment.IsDevelopment())
                {
                    _logger.LogWarning(
                        "SMTP no configurado. Correo NO enviado (modo desarrollo). Para: {To} | Asunto: {Subject}{NewLine}{Body}",
                        toEmail, subject, Environment.NewLine, htmlBody);
                    return;
                }

                throw new InvalidOperationException("El servicio de correo no está configurado.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            // 465 = SSL directo; cualquier otro puerto (587) = STARTTLS.
            var security = _settings.Port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, security, cancellationToken);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
