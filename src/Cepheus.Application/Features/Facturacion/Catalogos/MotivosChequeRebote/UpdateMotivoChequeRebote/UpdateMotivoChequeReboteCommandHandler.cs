using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.UpdateMotivoChequeRebote
{
    public class UpdateMotivoChequeReboteCommandHandler
        : IRequestHandler<UpdateMotivoChequeReboteCommand, MotivoChequeReboteResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMotivoChequeReboteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoChequeReboteResponse> Handle(
            UpdateMotivoChequeReboteCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var current = await _uow.Facturacion.Catalogos.MotivosChequeRebote.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Motivo de rebote de cheque {request.Code} no encontrado.");
            }

            var motivo = new MotivoChequeRebote
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,
                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.MotivosChequeRebote.Update(motivo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El motivo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await MotivoChequeReboteProjection.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
