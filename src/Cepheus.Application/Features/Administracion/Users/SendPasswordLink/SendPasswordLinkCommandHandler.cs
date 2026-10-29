using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.SendPasswordLink
{
    public class SendPasswordLinkCommandHandler : IRequestHandler<SendPasswordLinkCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserInvitationService _invitationService;

        public SendPasswordLinkCommandHandler(IUnitOfWork uow, IUserInvitationService invitationService)
        {
            _uow = uow;
            _invitationService = invitationService;
        }

        public async Task Handle(SendPasswordLinkCommand request, CancellationToken cancellationToken)
        {
            var user = await _uow.Administracion.Users.Query()
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            if (!user.IsActive)
            {
                throw new InvalidOperationException("El usuario está inactivo. Actívalo antes de enviarle el enlace.");
            }

            // Se invalidan los enlaces anteriores y se envía uno nuevo (vale lo mismo que una invitación).
            var enviado = await _invitationService.SendAsync(user, cancellationToken);

            if (!enviado)
            {
                throw new InvalidOperationException("No se pudo enviar el correo. Revisa la configuración de correo e inténtalo de nuevo.");
            }
        }
    }
}
