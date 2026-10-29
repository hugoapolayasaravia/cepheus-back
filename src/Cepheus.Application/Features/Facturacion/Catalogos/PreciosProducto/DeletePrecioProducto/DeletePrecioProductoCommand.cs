using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.DeletePrecioProducto
{
    public record DeletePrecioProductoCommand(
        string FleteCode,
        string ProductoTipoCode,
        string ProductoCode,
        string CurrencyTypeCode,
        string CurrencyCode
    ) : IRequest;
}
