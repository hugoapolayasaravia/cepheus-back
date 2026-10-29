using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.UpdateListaPrecio
{
    public class UpdateListaPrecioCommandHandler : IRequestHandler<UpdateListaPrecioCommand, ListaPrecioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateListaPrecioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ListaPrecioResponse> Handle(UpdateListaPrecioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.ListasPrecio.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Lista de precio {request.Id} no encontrada.");
            }

            var entity = new ListaPrecio
            {
                Id = request.Id,
                TipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant(),
                ProductoCode = request.ProductoCode.Trim().ToUpperInvariant(),
                Precio = request.Precio,
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.ListasPrecio.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La lista de precio fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateListaPrecio.CreateListaPrecioCommandHandler.Map(entity);
        }
    }
}
