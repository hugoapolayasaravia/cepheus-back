using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.ToggleProveedorContactoStatus
{
    public record ToggleProveedorContactoStatusCommand(int Id) : IRequest<bool>;
}
