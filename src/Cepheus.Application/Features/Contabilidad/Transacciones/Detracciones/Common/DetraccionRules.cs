using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common
{
    public static class DetraccionRules
    {
        public const int LotLength = 4;

        /// <summary>Comprobantes que generan detracción: factura y nota de débito (legacy: Codigo_tdo 01 y 08).</summary>
        public const string ComprobanteFactura = "01";
        public const string ComprobanteNotaDebito = "08";

        public const int OperacionMaxLength = 10;
        public const int ConstanciaMaxLength = 10;
        public const int ComprobantePagoMaxLength = 25;

        private static readonly TimeZoneInfo PeruTimeZone = ResolvePeruTimeZone();

        private static TimeZoneInfo ResolvePeruTimeZone()
        {
            foreach (var id in new[] { "America/Lima", "SA Pacific Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            // Perú no tiene horario de verano: UTC-5 fijo.
            return TimeZoneInfo.CreateCustomTimeZone("PET", TimeSpan.FromHours(-5), "Peru", "Peru");
        }

        /// <summary>Fecha de hoy en Perú (la auditoría guarda CreatedAt en UTC).</summary>
        public static DateTime TodayLocal()
            => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PeruTimeZone).Date;

        /// <summary>Instante UTC en que empieza el día (hora de Perú) indicado.</summary>
        public static DateTime LocalDayStartUtc(DateTime localDate)
            => TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(localDate.Date, DateTimeKind.Unspecified), PeruTimeZone);

        public static string Normalize(string value) => value.Trim();

        public static string Code(string year, string period, string lot) => year + period + lot;

        public static string Label(string year, string period, string lot) => $"{year}-{period}-{lot}";

        public static string NotFoundMessage(string year, string period, string lot)
            => $"Detracción {Label(year, period, lot)} no encontrada.";

        public static bool IsValidYear(string? year)
            => year is { Length: 4 } && year.All(char.IsDigit);

        public static bool IsValidPeriod(string? period)
            => period is { Length: 2 } && period.All(char.IsDigit)
               && int.Parse(period) is >= 1 and <= 12;

        public static bool IsValidLot(string? lot)
            => lot is { Length: LotLength } && lot.All(char.IsDigit);

        /// <summary>
        /// Detracción de un documento: total en soles x tasa (porcentaje 0-100) del tipo de bien,
        /// redondeada al sol (legacy: ROUND(TotalS_dco * POR_TASA_BIEN, 0)).
        /// </summary>
        public static decimal ComputeAmount(decimal totalSol, decimal ratePercent)
            => Math.Round(totalSol * ratePercent / 100m, 0, MidpointRounding.AwayFromZero);

        public static bool TryParseStatus(string? value, out EstadoDetraccion status)
        {
            status = default;
            var text = value?.Trim();

            return !string.IsNullOrEmpty(text)
                   && System.Enum.TryParse(text, true, out status)
                   && System.Enum.IsDefined(typeof(EstadoDetraccion), status);
        }

        public static string StatusNames => string.Join(", ", System.Enum.GetNames(typeof(EstadoDetraccion)));

        /// <summary>Recalcula el depósito de cada ítem (solo documentos marcados para exportar) y el total del lote.</summary>
        public static void Recalculate(Detraccion detraccion)
        {
            foreach (var item in detraccion.Items)
            {
                item.DepositSol = item.Documents
                    .Where(d => d.IncludeInExport)
                    .Sum(d => d.DetraccionSol);
            }

            detraccion.TotalSol = detraccion.Items.Sum(i => i.DepositSol);
        }

        public static void EnsurePendiente(Detraccion detraccion, string action)
        {
            if (detraccion.Status != EstadoDetraccion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"Solo se puede {action} un lote Pendiente. El lote está {detraccion.Status}.");
            }
        }
    }
}
