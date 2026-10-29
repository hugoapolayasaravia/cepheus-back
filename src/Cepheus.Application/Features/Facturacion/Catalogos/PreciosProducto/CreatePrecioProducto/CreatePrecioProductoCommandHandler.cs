using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.CreatePrecioProducto
{
    public class CreatePrecioProductoCommandHandler : IRequestHandler<CreatePrecioProductoCommand, PrecioProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePrecioProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PrecioProductoResponse> Handle(CreatePrecioProductoCommand request, CancellationToken cancellationToken)
        {
            var precioProducto = new PrecioProducto
            {
                FleteCode = request.FleteCode,
                ProductoTipoCode = request.ProductoTipoCode,
                ProductoCode = request.ProductoCode,
                CurrencyTypeCode = request.CurrencyTypeCode,
                CurrencyCode = request.CurrencyCode,
                Amount = request.Amount,
                TransportAmount = request.TransportAmount,
                FreightAmount = request.FreightAmount
            };

            await _uow.Facturacion.Catalogos.PreciosProducto.AddAsync(precioProducto, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(precioProducto);
        }

        internal static PrecioProductoResponse Map(PrecioProducto p) => new()
        {
            FleteCode = p.FleteCode,
            ProductoTipoCode = p.ProductoTipoCode,
            ProductoCode = p.ProductoCode,
            CurrencyTypeCode = p.CurrencyTypeCode,
            CurrencyCode = p.CurrencyCode,
            Amount = p.Amount,
            TransportAmount = p.TransportAmount,
            FreightAmount = p.FreightAmount,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            RowVersion = p.RowVersion
        };
    }
}
