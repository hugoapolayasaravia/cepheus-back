using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.GetFormaPagoVentaByCode
{
    public class GetFormaPagoVentaByCodeQueryHandler : IRequestHandler<GetFormaPagoVentaByCodeQuery, FormaPagoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetFormaPagoVentaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoVentaResponse> Handle(GetFormaPagoVentaByCodeQuery request, CancellationToken cancellationToken)
        {
            var formaPagoVenta = await _uow.Facturacion.Catalogos.FormasPagoVenta.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new FormaPagoVentaResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    Days = t.Days,
                    IsCredit = t.IsCredit,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (formaPagoVenta is null)
            {
                throw new KeyNotFoundException($"Forma de pago de ventas {request.Code} no encontrada.");
            }

            return formaPagoVenta;
        }
    }
}
