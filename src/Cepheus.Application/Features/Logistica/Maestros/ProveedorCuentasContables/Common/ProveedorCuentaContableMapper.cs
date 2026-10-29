using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common
{
    public static class ProveedorCuentaContableMapper
    {
        public static ProveedorCuentaContableResponse Map(ProveedorCuentaContable e) => new()
        {
            ProveedorCode = e.ProveedorCode,
            ProveedorName = e.Proveedor?.LegalName,
            MonedaCode = e.MonedaCode,
            AccountingAccountCode = e.AccountingAccountCode,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
