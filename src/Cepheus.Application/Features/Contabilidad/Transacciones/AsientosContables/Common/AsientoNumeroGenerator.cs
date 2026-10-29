using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    /// <summary>
    /// Correlativo del asiento por año, mes y tipo (legacy: fc_creanumero = máximo + 1, 5 dígitos).
    /// Para evitar duplicados con usuarios simultáneos, el proceso que lo usa debe tomar primero el bloqueo
    /// de aplicación <see cref="LockResource"/> dentro de su transacción.
    /// </summary>
    public static class AsientoNumeroGenerator
    {
        public static string LockResource(string anio, string mes, string tipoAsientoCode)
            => $"asiento:{anio}{mes}{tipoAsientoCode}";

        public static async Task<string> NextAsync(
            IUnitOfWork uow, string anio, string mes, string tipoAsientoCode, CancellationToken cancellationToken)
        {
            var last = await uow.Contabilidad.Transacciones.AsientosContables.Query()
                .AsNoTracking()
                .Where(a => a.Anio == anio && a.Mes == mes && a.TipoAsientoCode == tipoAsientoCode)
                .OrderByDescending(a => a.Numero)
                .Select(a => a.Numero)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (last is not null && int.TryParse(last, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            if (next > 99999)
            {
                throw new InvalidOperationException(
                    $"Se agotó el correlativo de asientos {tipoAsientoCode} del período {anio}-{mes}.");
            }

            return next.ToString().PadLeft(AsientoContableRules.NumeroLength, '0');
        }
    }
}
