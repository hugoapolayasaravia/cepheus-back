using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedoresByProveedor
{
    public record GetArticuloProveedoresByProveedorQuery(string ProveedorCode) : IRequest<List<ArticuloProveedorResponse>>;

}
