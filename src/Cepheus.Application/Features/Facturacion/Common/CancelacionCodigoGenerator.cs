using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Common
{
    /// <summary>
    /// Correlativo de cancelaciones (legacy: max(convert(int, Codigo_Can)) + 1, 10 dígitos). Global, no por planta.
    /// Debe llamarse dentro de una transacción: toma un bloqueo para que dos usuarios no obtengan el mismo número.
    /// </summary>
    public static class CancelacionCodigoGenerator
    {
        public const string LockResource = "cancelacion-codigo";

        public static async Task<string> NextAsync(IUnitOfWork uow, CancellationToken cancellationToken)
        {
            await uow.AcquireAppLockAsync(LockResource, 15000, cancellationToken);

            var last = await uow.Facturacion.Transacciones.CancelacionesVenta.Query().AsNoTracking()
                .OrderByDescending(c => c.Code)
                .Select(c => c.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = long.TryParse(last?.Trim(), out var n) ? n + 1 : 1;

            if (next > 9_999_999_999)
            {
                throw new InvalidOperationException("Se agotó la numeración de cancelaciones.");
            }

            return next.ToString().PadLeft(10, '0');
        }
    }
}
