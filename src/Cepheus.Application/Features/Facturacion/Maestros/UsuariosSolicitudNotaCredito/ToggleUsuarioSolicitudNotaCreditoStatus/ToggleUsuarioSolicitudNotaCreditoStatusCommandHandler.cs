using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.ToggleUsuarioSolicitudNotaCreditoStatus
{
    public class ToggleUsuarioSolicitudNotaCreditoStatusCommandHandler
        : IRequestHandler<ToggleUsuarioSolicitudNotaCreditoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleUsuarioSolicitudNotaCreditoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(
            ToggleUsuarioSolicitudNotaCreditoStatusCommand request, CancellationToken cancellationToken)
        {
            var negocioCode = request.NegocioCode.Trim().ToUpperInvariant();
            var stage = EtapaSolicitudNotaCreditoParser.Parse(request.Stage);

            var usuario = await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                .FirstOrDefaultAsync(u => u.NegocioCode == negocioCode && u.Stage == stage && u.UserId == request.UserId,
                    cancellationToken);

            if (usuario is null)
            {
                throw new KeyNotFoundException(
                    $"Usuario {request.UserId} no está asignado a la etapa {stage} del negocio {negocioCode}.");
            }

            if (!usuario.IsActive && usuario.IsPrincipal)
            {
                var otherPrincipal = await _uow.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Query()
                    .AnyAsync(u => u.NegocioCode == negocioCode && u.Stage == stage && u.UserId != request.UserId
                                   && u.IsPrincipal && u.IsActive, cancellationToken);

                if (otherPrincipal)
                {
                    throw new InvalidOperationException(
                        "No se puede activar: ya existe otro usuario principal activo para esa etapa y negocio.");
                }
            }

            usuario.IsActive = !usuario.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return usuario.IsActive;
        }
    }
}
