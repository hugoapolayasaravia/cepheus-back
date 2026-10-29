using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.UpdateFormaPago
{
    public class UpdateFormaPagoCommandHandler : IRequestHandler<UpdateFormaPagoCommand, FormaPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFormaPagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoResponse> Handle(UpdateFormaPagoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.FormasPago.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Forma de pago {request.Code} no encontrada.");
            }

            var formaPago = new FormaPago
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                Days = request.Days,
                IsCredit = request.IsCredit,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.FormasPago.Update(formaPago);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La forma de pago fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new FormaPagoResponse
            {
                Code = formaPago.Code,
                Name = formaPago.Name,
                Days = formaPago.Days,
                IsCredit = formaPago.IsCredit,
                IsActive = formaPago.IsActive,
                CreatedAt = formaPago.CreatedAt,
                UpdatedAt = formaPago.UpdatedAt,
                CreatedBy = formaPago.CreatedBy,
                UpdatedBy = formaPago.UpdatedBy,
                RowVersion = formaPago.RowVersion
            };
        }
    }
}