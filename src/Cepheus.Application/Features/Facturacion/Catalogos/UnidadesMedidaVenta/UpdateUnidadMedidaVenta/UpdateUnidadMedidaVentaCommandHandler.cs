using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.UpdateUnidadMedidaVenta
{
    public class UpdateUnidadMedidaVentaCommandHandler : IRequestHandler<UpdateUnidadMedidaVentaCommand, UnidadMedidaVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadMedidaVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadMedidaVentaResponse> Handle(UpdateUnidadMedidaVentaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Unidad de medida de venta {request.Code} no encontrado.");
            }

            var entity = new UnidadMedidaVenta
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateUnidadMedidaVenta.CreateUnidadMedidaVentaCommandHandler.Map(entity);
        }
    }
}
