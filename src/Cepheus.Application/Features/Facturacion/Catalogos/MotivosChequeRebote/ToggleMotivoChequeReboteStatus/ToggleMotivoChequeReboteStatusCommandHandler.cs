using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.ToggleMotivoChequeReboteStatus
{
    public class ToggleMotivoChequeReboteStatusCommandHandler
        : IRequestHandler<ToggleMotivoChequeReboteStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public ToggleMotivoChequeReboteStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(ToggleMotivoChequeReboteStatusCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var motivo = await _uow.Facturacion.Catalogos.MotivosChequeRebote.Query()
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

            if (motivo is null)
            {
                throw new KeyNotFoundException($"Motivo de rebote de cheque {request.Code} no encontrado.");
            }

            motivo.IsActive = !motivo.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return motivo.IsActive;
        }
    }
}
