using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common
{
    public sealed record DetraccionGenerated(List<DetraccionItem> Items, List<string> Warnings);

    /// <summary>
    /// Arma los ítems y documentos de un lote de detracción (legacy: SP com_add_upd_detracion).
    ///
    /// Documentos que entran: facturas y notas de débito (01 y 08) procesadas en el rango de fechas,
    /// con total distinto de cero, ya provisionadas (con asiento de provisión), con tipo de bien y tipo de
    /// operación, y que no estén en otro lote. La detracción de cada documento es total x tasa del tipo
    /// de bien, redondeada al sol. Los documentos se agrupan por proveedor + tipo de operación + tipo de
    /// bien: cada grupo es un ítem de depósito.
    ///
    /// Cambios frente al legacy:
    ///   - El rango se compara por día completo en hora de Perú (el legacy dejaba fuera los documentos
    ///     procesados el último día porque FEC_FIN llevaba hora 00:00).
    ///   - Los documentos con detracción calculada en 0 quedan fuera (se avisa) en vez de generar
    ///     una línea de depósito en cero.
    ///   - Si un proveedor no tiene RUC válido o cuenta de detracciones, se informa cuáles son en lugar de
    ///     fallar con un error de base de datos.
    /// </summary>
    public static class DetraccionGenerator
    {
        private const int MaxListed = 10;

        public static async Task<DetraccionGenerated> GenerateAsync(
            IUnitOfWork uow,
            IDetraccionSettings settings,
            string year,
            string period,
            string lot,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken)
        {
            var bancoCode = settings.BancoCode?.Trim();

            if (string.IsNullOrEmpty(bancoCode))
            {
                throw new InvalidOperationException(
                    "Falta configurar Detraccion:BancoCode (código del Banco de la Nación) en appsettings.");
            }

            var from = DetraccionRules.LocalDayStartUtc(startDate);
            var toExclusive = DetraccionRules.LocalDayStartUtc(endDate.Date.AddDays(1));

            var assigned = uow.Contabilidad.Transacciones.DetraccionDocumentos.Query();

            var docs = await uow.Contabilidad.Transacciones.DocumentosCompra.Query().AsNoTracking()
                .Where(d => d.CreatedAt >= from && d.CreatedAt < toExclusive)
                .Where(d => d.ComprobantePagoCode == DetraccionRules.ComprobanteFactura
                            || d.ComprobantePagoCode == DetraccionRules.ComprobanteNotaDebito)
                .Where(d => d.TotalSol != 0m)
                .Where(d => d.AsientoProvisionCode != null && d.AsientoProvisionCode.Trim().Length == 13)
                .Where(d => d.TipoBienCode != null && d.TipoOperacionCode != null)
                .Where(d => !assigned.Any(x => x.ComprobantePagoCode == d.ComprobantePagoCode
                                               && x.NumeroDocumento == d.NumeroDocumento
                                               && x.ProveedorCode == d.ProveedorCode))
                .Select(d => new
                {
                    d.ComprobantePagoCode,
                    d.NumeroDocumento,
                    d.ProveedorCode,
                    TipoOperacionCode = d.TipoOperacionCode!,
                    TipoBienCode = d.TipoBienCode!,
                    d.TotalSol,
                    Rate = d.TipoBien!.DetractionRate
                })
                .ToListAsync(cancellationToken);

            var priced = docs
                .Select(d => new { Doc = d, Amount = DetraccionRules.ComputeAmount(d.TotalSol, d.Rate) })
                .ToList();

            var kept = priced.Where(x => x.Amount > 0m).ToList();
            var zeroCount = priced.Count - kept.Count;

            if (kept.Count == 0)
            {
                var message =
                    "No existen documentos de compra (factura o nota de débito, provisionados, con tipo de bien " +
                    "y tipo de operación) en este rango de fechas que no estén ya en otro lote de detracción.";

                if (zeroCount > 0)
                {
                    message += $" Se encontraron {zeroCount} documento(s) pero su detracción calculada es 0.";
                }

                throw new InvalidOperationException(message);
            }

            var warnings = new List<string>();

            if (zeroCount > 0)
            {
                warnings.Add(
                    $"{zeroCount} documento(s) quedaron fuera del lote porque su detracción calculada es 0 " +
                    "(tasa en 0 o importe menor a medio sol).");
            }

            var provCodes = kept.Select(x => x.Doc.ProveedorCode).Distinct().ToList();

            var providers = await uow.Logistica.Maestros.Proveedores.Query().AsNoTracking()
                .Where(p => provCodes.Contains(p.Code))
                .Select(p => new { p.Code, p.DocumentNumber, p.LegalName })
                .ToListAsync(cancellationToken);

            var accounts = await uow.Logistica.Maestros.ProveedorCuentas.Query().AsNoTracking()
                .Where(c => provCodes.Contains(c.ProveedorCode) && c.IsActive && c.BancoCode == bancoCode)
                .Select(c => new { c.Id, c.ProveedorCode, c.AccountNumber, c.MonedaCode, c.IsPrimary })
                .ToListAsync(cancellationToken);

            var withoutRuc = new List<string>();
            var withoutAccount = new List<string>();
            var providerData = new Dictionary<string, (string Ruc, int AccountId, string Account)>();

            foreach (var code in provCodes)
            {
                var provider = providers.FirstOrDefault(p => p.Code == code);
                var label = provider is null ? code : $"{code} {provider.LegalName.Trim()}";
                var ruc = provider?.DocumentNumber?.Trim();

                if (string.IsNullOrEmpty(ruc) || ruc.Length != 11 || !ruc.All(char.IsDigit))
                {
                    withoutRuc.Add(label);
                    continue;
                }

                var account = accounts
                    .Where(a => a.ProveedorCode == code && a.AccountNumber.Any(char.IsDigit))
                    .OrderByDescending(a => TipoCambioRules.EsMonedaBase(a.MonedaCode))
                    .ThenByDescending(a => a.IsPrimary)
                    .ThenBy(a => a.Id)
                    .FirstOrDefault();

                if (account is null)
                {
                    withoutAccount.Add(label);
                    continue;
                }

                providerData[code] = (ruc, account.Id, account.AccountNumber.Trim());
            }

            if (withoutRuc.Count > 0 || withoutAccount.Count > 0)
            {
                var parts = new List<string>();

                if (withoutRuc.Count > 0)
                {
                    parts.Add("proveedores sin RUC válido de 11 dígitos: " + Listed(withoutRuc));
                }

                if (withoutAccount.Count > 0)
                {
                    parts.Add("proveedores sin cuenta de detracciones activa en el Banco de la Nación: "
                              + Listed(withoutAccount));
                }

                throw new InvalidOperationException("No se puede generar el lote: " + string.Join("; ", parts) + ".");
            }

            var groups = kept
                .GroupBy(x => (x.Doc.ProveedorCode, x.Doc.TipoOperacionCode, x.Doc.TipoBienCode))
                .OrderBy(g => g.Key.TipoOperacionCode, StringComparer.Ordinal)
                .ThenBy(g => g.Key.TipoBienCode, StringComparer.Ordinal)
                .ThenBy(g => g.Key.ProveedorCode, StringComparer.Ordinal)
                .ToList();

            var items = new List<DetraccionItem>();
            var itemNumber = 0;

            foreach (var group in groups)
            {
                itemNumber++;
                var data = providerData[group.Key.ProveedorCode];

                var item = new DetraccionItem
                {
                    Year = year,
                    Period = period,
                    LotNumber = lot,
                    ProveedorCode = group.Key.ProveedorCode,
                    ItemNumber = itemNumber,
                    TipoOperacionCode = group.Key.TipoOperacionCode,
                    TipoBienCode = group.Key.TipoBienCode,
                    ProveedorRuc = data.Ruc,
                    AccountNumber = data.Account,
                    ProveedorCuentaId = data.AccountId,
                    DepositSol = group.Sum(x => x.Amount)
                };

                foreach (var entry in group
                             .OrderBy(x => x.Doc.ComprobantePagoCode, StringComparer.Ordinal)
                             .ThenBy(x => x.Doc.NumeroDocumento, StringComparer.Ordinal))
                {
                    item.Documents.Add(new DetraccionDocumento
                    {
                        Year = year,
                        Period = period,
                        LotNumber = lot,
                        ProveedorCode = item.ProveedorCode,
                        ItemNumber = itemNumber,
                        ComprobantePagoCode = entry.Doc.ComprobantePagoCode,
                        NumeroDocumento = entry.Doc.NumeroDocumento,
                        DetraccionSol = entry.Amount,
                        IncludeInExport = true
                    });
                }

                items.Add(item);
            }

            return new DetraccionGenerated(items, warnings);
        }

        private static string Listed(List<string> values)
        {
            var text = string.Join(", ", values.Take(MaxListed));
            return values.Count > MaxListed ? $"{text} y {values.Count - MaxListed} más" : text;
        }
    }
}
