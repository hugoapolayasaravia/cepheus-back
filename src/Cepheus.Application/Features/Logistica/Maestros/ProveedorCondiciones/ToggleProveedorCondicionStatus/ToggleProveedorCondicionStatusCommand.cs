using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.ToggleProveedorCondicionStatus
{
    public record ToggleProveedorCondicionStatusCommand(int Id) : IRequest<bool>;
}
