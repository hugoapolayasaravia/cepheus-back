using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.DeleteArticuloProveedor
{
    public record DeleteArticuloProveedorCommand(
        string PlantaCode,
        string ArticuloCode,
        string ProveedorCode
    ) : IRequest;
}
