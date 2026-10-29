using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.ToggleProveedorDireccionStatus
{
    public record ToggleProveedorDireccionStatusCommand(int Id) : IRequest<bool>;
}
