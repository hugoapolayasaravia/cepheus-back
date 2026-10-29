using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.UpdateTipoProducto
{
    public class UpdateTipoProductoCommandHandler : IRequestHandler<UpdateTipoProductoCommand, TipoProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoProductoResponse> Handle(UpdateTipoProductoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.TiposProducto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de producto {request.Code} no encontrado.");
            }

            var entity = new TipoProducto
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.TiposProducto.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTipoProducto.CreateTipoProductoCommandHandler.Map(entity);
        }
    }
}
