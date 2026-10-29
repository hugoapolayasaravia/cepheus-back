using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.ToggleProveedorStatus
{
    public record ToggleProveedorStatusCommand(string Code) : IRequest<bool>;
}
