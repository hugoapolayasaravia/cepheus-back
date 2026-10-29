using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.ForgotPassword
{
    /// <summary>
    /// Solicita el enlace de restablecimiento. No devuelve nada a propósito: la respuesta
    /// es la misma exista o no el correo (evita enumeración de usuarios).
    /// </summary>
    public record ForgotPasswordCommand(string Email) : IRequest;
}
