using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common
{
    /// <summary>
    /// El documento guarda solo la FECHA del tipo de cambio; el valor se consulta en Comunes.TipoCambio para la moneda del documento.
    /// Soles -> 1. Sin tipo de cambio registrado en esa fecha -> null.
    /// </summary>
    public static class DocumentoCompraTipoCambioResolver
    {
        public static async Task<decimal?> GetVentaAsync(
            IUnitOfWork uow, string monedaCode, DateOnly? fecha, CancellationToken ct)
        {
            if (string.Equals(monedaCode?.Trim(), DocumentoCompraRules.MonedaSoles, StringComparison.OrdinalIgnoreCase))
            {
                return 1m;
            }

            if (fecha is null)
            {
                return null;
            }

            return await TipoCambioRules.GetVentaAsync(uow, monedaCode!, fecha.Value, ct);
        }
    }
}
