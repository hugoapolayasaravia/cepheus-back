using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.UpdateProveedorCuentaContable
{
    /// <summary>La clave (proveedor + moneda) no cambia; se actualiza la cuenta.</summary>
    public record UpdateProveedorCuentaContableCommand(
        string ProveedorCode,
        string MonedaCode,
        string AccountingAccountCode,
        byte[] RowVersion
    ) : IRequest<ProveedorCuentaContableResponse>, IProveedorCuentaContableDatos;
}
