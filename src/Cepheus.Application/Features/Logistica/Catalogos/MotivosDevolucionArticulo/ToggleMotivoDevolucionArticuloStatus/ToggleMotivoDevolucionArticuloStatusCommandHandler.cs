using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.ToggleMotivoDevolucionArticuloStatus
{
    public class ToggleMotivoDevolucionArticuloStatusCommandHandler : IRequestHandler<ToggleMotivoDevolucionArticuloStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleMotivoDevolucionArticuloStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleMotivoDevolucionArticuloStatusCommand request, CancellationToken cancellationToken)
        {
            var motivo = await _uow.Logistica.Catalogos.MotivosDevolucionArticulo.GetByCodeAsync(request.Code, cancellationToken);

            if (motivo is null)
            {
                throw new KeyNotFoundException($"Motivo de devolución {request.Code} no encontrado.");
            }

            motivo.IsActive = !motivo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return motivo.IsActive;
        }
    }
}
