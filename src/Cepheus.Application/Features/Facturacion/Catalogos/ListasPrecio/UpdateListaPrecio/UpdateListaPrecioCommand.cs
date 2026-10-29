using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.UpdateListaPrecio
{
    public record UpdateListaPrecioCommand(
        long Id,
        string TipoProductoCode,
        string ProductoCode,
        decimal Precio,
        DateTime FechaInicio,
        DateTime? FechaFin,
        byte[] RowVersion
    ) : IRequest<ListaPrecioResponse>;
}
