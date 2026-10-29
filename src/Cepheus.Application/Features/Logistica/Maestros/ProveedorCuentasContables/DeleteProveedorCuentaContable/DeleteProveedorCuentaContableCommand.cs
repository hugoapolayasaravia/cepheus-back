using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.DeleteProveedorCuentaContable
{
    public record DeleteProveedorCuentaContableCommand(string ProveedorCode, string MonedaCode) : IRequest;
}
