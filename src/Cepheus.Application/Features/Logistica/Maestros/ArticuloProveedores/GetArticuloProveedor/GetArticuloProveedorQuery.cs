using Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.GetArticuloProveedor
{
    public record GetArticuloProveedorQuery(
        string PlantaCode,
        string ArticuloCode,
        string ProveedorCode
    ) : IRequest<ArticuloProveedorResponse>;
}

