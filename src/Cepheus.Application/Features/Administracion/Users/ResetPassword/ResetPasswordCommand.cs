using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.ResetPassword
{
    /// <summary>
    /// Establece una contraseña nueva usando el token que llegó en el enlace del correo.
    /// </summary>
    public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;
}
