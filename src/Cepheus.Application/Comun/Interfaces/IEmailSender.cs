namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Envío de correos. La implementación (SMTP/Gmail) vive en Infrastructure.
    /// </summary>
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }
}
