using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio
{
    public class CreateListaPrecioCommandHandler : IRequestHandler<CreateListaPrecioCommand, ListaPrecioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateListaPrecioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ListaPrecioResponse> Handle(CreateListaPrecioCommand request, CancellationToken cancellationToken)
        {
            var entity = new ListaPrecio
            {
                TipoProductoCode = request.TipoProductoCode.Trim().ToUpperInvariant(),
                ProductoCode = request.ProductoCode.Trim().ToUpperInvariant(),
                Precio = request.Precio,
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.ListasPrecio.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static ListaPrecioResponse Map(ListaPrecio e) => new()
        {
            Id = e.Id,
            TipoProductoCode = e.TipoProductoCode,
            ProductoCode = e.ProductoCode,
            Precio = e.Precio,
            FechaInicio = e.FechaInicio,
            FechaFin = e.FechaFin,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
