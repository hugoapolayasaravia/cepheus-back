using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.UpdateMotivoDevolucion
{
    public class UpdateMotivoDevolucionCommandHandler : IRequestHandler<UpdateMotivoDevolucionCommand, MotivoDevolucionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMotivoDevolucionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<MotivoDevolucionResponse> Handle(UpdateMotivoDevolucionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.MotivosDevolucion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Motivo de devolución {request.Code} no encontrado.");
            }

            var motivo = new MotivoDevolucion
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                AffectsStock = request.AffectsStock,
                EsVenta = request.EsVenta,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.MotivosDevolucion.Update(motivo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El motivo de devolución fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new MotivoDevolucionResponse
            {
                Code = motivo.Code,
                Name = motivo.Name,
                AffectsStock = motivo.AffectsStock,
                EsVenta= motivo.EsVenta,
                IsActive = motivo.IsActive,
                CreatedAt = motivo.CreatedAt,
                UpdatedAt = motivo.UpdatedAt,
                CreatedBy = motivo.CreatedBy,
                UpdatedBy = motivo.UpdatedBy,
                RowVersion = motivo.RowVersion
            };
        }
    }
}
