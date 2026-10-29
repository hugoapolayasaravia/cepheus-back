using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.GetListaPrecioById
{
    public record GetListaPrecioByIdQuery(long Id) : IRequest<ListaPrecioResponse>;
}
