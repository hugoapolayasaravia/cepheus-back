using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.UpdateFormaPagoVenta
{
    public class UpdateFormaPagoVentaCommandHandler : IRequestHandler<UpdateFormaPagoVentaCommand, FormaPagoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFormaPagoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoVentaResponse> Handle(UpdateFormaPagoVentaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.FormasPagoVenta.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Forma de pago de ventas {request.Code} no encontrada.");
            }

            var formaPagoVenta = new FormaPagoVenta
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

            _uow.Facturacion.Catalogos.FormasPagoVenta.Update(formaPagoVenta);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La forma de pago de ventas fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new FormaPagoVentaResponse
            {
                Code = formaPagoVenta.Code,
                Name = formaPagoVenta.Name,
                Days = formaPagoVenta.Days,
                IsCredit = formaPagoVenta.IsCredit,
                IsActive = formaPagoVenta.IsActive,
                CreatedAt = formaPagoVenta.CreatedAt,
                UpdatedAt = formaPagoVenta.UpdatedAt,
                RowVersion = formaPagoVenta.RowVersion
            };
        }
    }
}
