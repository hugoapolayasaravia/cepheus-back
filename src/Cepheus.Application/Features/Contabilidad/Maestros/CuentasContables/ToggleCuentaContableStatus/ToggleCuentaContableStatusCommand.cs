using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.ToggleCuentaContableStatus
{
    public record ToggleCuentaContableStatusCommand(string Code) : IRequest<bool>;
}
