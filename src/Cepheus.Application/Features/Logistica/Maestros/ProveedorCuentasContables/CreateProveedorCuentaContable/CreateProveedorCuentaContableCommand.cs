using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.CreateProveedorCuentaContable
{
    public record CreateProveedorCuentaContableCommand(
        string ProveedorCode,
        string MonedaCode,
        string AccountingAccountCode
    ) : IRequest<ProveedorCuentaContableResponse>, IProveedorCuentaContableDatos;
}
