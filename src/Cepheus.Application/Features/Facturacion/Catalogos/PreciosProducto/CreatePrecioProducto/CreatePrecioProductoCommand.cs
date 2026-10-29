using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.CreatePrecioProducto
{
    public record CreatePrecioProductoCommand(
        string FleteCode,
        string ProductoTipoCode,
        string ProductoCode,
        string CurrencyTypeCode,
        string CurrencyCode,
        decimal Amount,
        decimal TransportAmount,
        decimal FreightAmount
    ) : IRequest<PrecioProductoResponse>;
}
