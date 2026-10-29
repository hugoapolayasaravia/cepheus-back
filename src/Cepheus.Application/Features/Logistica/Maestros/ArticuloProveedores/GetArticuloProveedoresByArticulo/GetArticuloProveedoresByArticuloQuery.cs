using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedoresByArticulo
{
    public record GetArticuloProveedoresByArticuloQuery(string ArticuloCode) : IRequest<List<ArticuloProveedorResponse>>;
}
