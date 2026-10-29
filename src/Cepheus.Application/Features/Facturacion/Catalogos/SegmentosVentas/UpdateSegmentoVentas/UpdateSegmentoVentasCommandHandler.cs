using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.UpdateSegmentoVentas
{
    public class UpdateSegmentoVentasCommandHandler : IRequestHandler<UpdateSegmentoVentasCommand, SegmentoVentasResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSegmentoVentasCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SegmentoVentasResponse> Handle(UpdateSegmentoVentasCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.SegmentosVentas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Segmento de ventas {request.Code} no encontrado.");
            }

            var segmentoVentas = new SegmentoVentas
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.SegmentosVentas.Update(segmentoVentas);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El segmento de ventas fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SegmentoVentasResponse
            {
                Code = segmentoVentas.Code,
                Name = segmentoVentas.Name,
                IsActive = segmentoVentas.IsActive,
                CreatedAt = segmentoVentas.CreatedAt,
                UpdatedAt = segmentoVentas.UpdatedAt,
                RowVersion = segmentoVentas.RowVersion
            };
        }
    }
}
