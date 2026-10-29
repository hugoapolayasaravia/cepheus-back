using Cepheus.Domain.Administracion;

namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Invitación de un usuario nuevo: crea un enlace de un solo uso para que defina su propia
    /// contraseña y se lo envía por correo. La implementación vive en Infrastructure.
    /// </summary>
    public interface IUserInvitationService
    {
        /// <summary>
        /// Devuelve true si el correo se envió. Un fallo de envío no lanza excepción: se registra en el
        /// log y se devuelve false, para que el usuario ya creado no se pierda.
        /// </summary>
        Task<bool> SendAsync(User user, CancellationToken cancellationToken);
    }
}
