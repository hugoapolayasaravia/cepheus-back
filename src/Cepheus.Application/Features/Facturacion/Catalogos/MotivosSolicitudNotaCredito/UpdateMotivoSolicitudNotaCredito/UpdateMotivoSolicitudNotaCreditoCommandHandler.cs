using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.UpdateMotivoSolicitudNotaCredito
{
    public class UpdateMotivoSolicitudNotaCreditoCommandHandler
        : IRequestHandler<UpdateMotivoSolicitudNotaCreditoCommand, MotivoSolicitudNotaCreditoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMotivoSolicitudNotaCreditoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoSolicitudNotaCreditoResponse> Handle(
            UpdateMotivoSolicitudNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var current = await _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Motivo de solicitud de nota de crédito {request.Code} no encontrado.");
            }

            var motivo = new MotivoSolicitudNotaCredito
            {
                Code = code,
                Name = request.Name.Trim(),
                ParentCode = string.IsNullOrWhiteSpace(request.ParentCode) ? null : request.ParentCode.Trim(),
                MotivoDevolucionCode = string.IsNullOrWhiteSpace(request.MotivoDevolucionCode) ? null : request.MotivoDevolucionCode.Trim(),
                AffectsStock = request.AffectsStock,
                StartDate = request.StartDate,
                EndDate = request.EndDate,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Update(motivo);

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

            return (await MotivoSolicitudNotaCreditoProjection.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
