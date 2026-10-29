using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio
{
    public record CreateListaPrecioCommand(
        string TipoProductoCode,
        string ProductoCode,
        decimal Precio,
        DateTime FechaInicio,
        DateTime? FechaFin
    ) : IRequest<ListaPrecioResponse>;
}
