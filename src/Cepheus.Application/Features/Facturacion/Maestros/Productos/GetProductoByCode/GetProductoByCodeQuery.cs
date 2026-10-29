using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.GetProductoByCode
{
    public record GetProductoByCodeQuery(string TipoProductoCode, string Code) : IRequest<ProductoResponse>;
}
