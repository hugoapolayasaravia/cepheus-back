using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.UpdateAnalisisVenta
{
    public class UpdateAnalisisVentaCommandHandler : IRequestHandler<UpdateAnalisisVentaCommand, AnalisisVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAnalisisVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AnalisisVentaResponse> Handle(UpdateAnalisisVentaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.AnalisisVentas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Análisis de ventas {request.Code} no encontrado.");
            }

            var analisisVenta = new AnalisisVenta
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim(),
                SegmentoVentasCode = string.IsNullOrWhiteSpace(request.SegmentoVentasCode) ? null : request.SegmentoVentasCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.AnalisisVentas.Update(analisisVenta);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El análisis de ventas fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new AnalisisVentaResponse
            {
                Code = analisisVenta.Code,
                Name = analisisVenta.Name,
                ShortName = analisisVenta.ShortName,
                SegmentoVentasCode = analisisVenta.SegmentoVentasCode,
                IsActive = analisisVenta.IsActive,
                CreatedAt = analisisVenta.CreatedAt,
                UpdatedAt = analisisVenta.UpdatedAt,
                RowVersion = analisisVenta.RowVersion
            };
        }
    }
}
