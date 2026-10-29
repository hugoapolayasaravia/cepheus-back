using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.ToggleMotivoSolicitudNotaCreditoStatus
{
    public class ToggleMotivoSolicitudNotaCreditoStatusCommandHandler
        : IRequestHandler<ToggleMotivoSolicitudNotaCreditoStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleMotivoSolicitudNotaCreditoStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(
            ToggleMotivoSolicitudNotaCreditoStatusCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var motivo = await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

            if (motivo is null)
            {
                throw new KeyNotFoundException($"Motivo de solicitud de nota de crédito {request.Code} no encontrado.");
            }

            motivo.IsActive = !motivo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return motivo.IsActive;
        }
    }
}
