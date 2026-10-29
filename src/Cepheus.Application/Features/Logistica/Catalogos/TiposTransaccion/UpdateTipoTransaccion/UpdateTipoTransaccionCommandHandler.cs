using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.UpdateTipoTransaccion
{
    public class UpdateTipoTransaccionCommandHandler : IRequestHandler<UpdateTipoTransaccionCommand, TipoTransaccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoTransaccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoTransaccionResponse> Handle(UpdateTipoTransaccionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.TiposTransaccion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de transacción {request.Code} no encontrado.");
            }

            var tipoTransaccion = new TipoTransaccion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.TiposTransaccion.Update(tipoTransaccion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de transacción fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoTransaccionResponse
            {
                Code = tipoTransaccion.Code,
                Name = tipoTransaccion.Name,
                IsActive = tipoTransaccion.IsActive,
                CreatedAt = tipoTransaccion.CreatedAt,
                UpdatedAt = tipoTransaccion.UpdatedAt,
                CreatedBy = tipoTransaccion.CreatedBy,
                UpdatedBy = tipoTransaccion.UpdatedBy,
                RowVersion = tipoTransaccion.RowVersion
            };
        }
    }
}
