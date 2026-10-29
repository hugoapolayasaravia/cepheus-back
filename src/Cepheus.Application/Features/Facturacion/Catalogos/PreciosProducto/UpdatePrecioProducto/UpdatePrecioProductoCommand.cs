using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.UpdatePrecioProducto
{
    /// <summary>
    /// Actualiza los montos de un precio existente. La clave compuesta
    /// (Flete/Producto/Moneda) no se puede modificar: para cambiarla hay que
    /// eliminar el registro y crear uno nuevo.
    /// </summary>
    public record UpdatePrecioProductoCommand(
        string FleteCode,
        string ProductoTipoCode,
        string ProductoCode,
        string CurrencyTypeCode,
        string CurrencyCode,
        decimal Amount,
        decimal TransportAmount,
        decimal FreightAmount,
        byte[] RowVersion
    ) : IRequest<PrecioProductoResponse>;
}
