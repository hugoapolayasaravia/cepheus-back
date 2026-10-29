using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    /// <summary>
    /// Cuenta contable por pagar (42/46) del proveedor para la moneda del documento (tabla ProveedorCuentaContable).
    /// Reemplaza a enlace1_cta (soles) / enlace2_cta (dólares) del legacy: se busca la fila del proveedor y la moneda
    /// del documento, para cualquier moneda. Si no existe, el proceso se rechaza con un mensaje claro.
    /// </summary>
    public static class ProveedorCuentaPorPagarResolver
    {
        public static async Task<string> ResolverAsync(
            IUnitOfWork uow, string proveedorCode, string monedaCode, CancellationToken cancellationToken)
        {
            var prv = proveedorCode.Trim();
            var mon = monedaCode.Trim().ToUpperInvariant();

            var cuenta = (await uow.Logistica.Maestros.ProveedorCuentasContables.Query()
                .AsNoTracking()
                .Where(c => c.ProveedorCode == prv && c.MonedaCode == mon)
                .Select(c => c.AccountingAccountCode)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty).Trim();

            if (cuenta.Length != 8)
            {
                throw new InvalidOperationException(
                    $"Proveedor {prv} no registra cuenta contable por pagar para la moneda {mon}");
            }

            if (!(cuenta.StartsWith("42") || cuenta.StartsWith("46")))
            {
                throw new InvalidOperationException(
                    $"Cta por pagar {mon} Errada .. <42/46> ... Reintente ...");
            }

            return cuenta;
        }
    }
}
