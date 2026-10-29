using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.GetPrecioProducto
{
    public record GetPrecioProductoQuery(
        string FleteCode,
        string ProductoTipoCode,
        string ProductoCode,
        string CurrencyTypeCode,
        string CurrencyCode
    ) : IRequest<PrecioProductoResponse>;
}
