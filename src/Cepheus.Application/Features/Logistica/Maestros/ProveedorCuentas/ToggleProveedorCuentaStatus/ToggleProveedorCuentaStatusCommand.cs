using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.ToggleProveedorCuentaStatus
{
    public record ToggleProveedorCuentaStatusCommand(int Id) : IRequest<bool>;
}
