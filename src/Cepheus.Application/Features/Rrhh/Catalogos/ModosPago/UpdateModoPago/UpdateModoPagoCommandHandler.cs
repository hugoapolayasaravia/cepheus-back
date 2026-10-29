using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.UpdateModoPago
{
    public class UpdateModoPagoCommandHandler : IRequestHandler<UpdateModoPagoCommand, ModoPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateModoPagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModoPagoResponse> Handle(UpdateModoPagoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.ModosPago.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Modo de pago {request.Code} no encontrado.");
            }

            var modoPago = new ModoPago
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.ModosPago.Update(modoPago);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El modo de pago fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ModoPagoResponse
            {
                Code = modoPago.Code,
                Name = modoPago.Name,
                IsActive = modoPago.IsActive,
                CreatedAt = modoPago.CreatedAt,
                UpdatedAt = modoPago.UpdatedAt,
                RowVersion = modoPago.RowVersion
            };
        }
    }
}