using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common
{
    public static class LetraVentaRules
    {
        private const string InitialNumber = "000-00000";

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static string FormatKey(string plantaCode, string number) => $"{plantaCode}/{number}";

        /// <summary>Legacy ue_set_btn_mod_dw: solo la letra por cobrar (Facturado) admite correcciones.</summary>
        public static bool CanEdit(LetraVenta l) => l.Status == EstadoLetra.Facturado;

        public static bool CanProtestar(LetraVenta l) => l.Status is EstadoLetra.Pendiente or EstadoLetra.Cancelado;

        public static string IncrementNumber(string? current)
        {
            var text = string.IsNullOrWhiteSpace(current) ? InitialNumber : current.Trim();
            var match = Regex.Match(text, @"^(?<prefix>.*?)(?<num>\d+)$");

            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"La numeración de letras de la planta ({text}) no tiene un formato válido (ej. {InitialNumber}).");
            }

            var digits = match.Groups["num"].Value;
            var next = (long.Parse(digits) + 1).ToString().PadLeft(digits.Length, '0');

            if (next.Length > digits.Length)
            {
                throw new InvalidOperationException("Se agotó la numeración de letras de la planta.");
            }

            return match.Groups["prefix"].Value + next;
        }

        /// <summary>
        /// Reserva <paramref name="count"/> números de letra de la planta (Planta.NumeroLet) bajo bloqueo y deja
        /// la planta apuntando al último. Debe llamarse dentro de una transacción.
        /// </summary>
        public static async Task<List<string>> ReserveNumbersAsync(
            IUnitOfWork uow, string plantaCode, int count, CancellationToken cancellationToken)
        {
            await uow.AcquireAppLockAsync($"letra-numero:{plantaCode}", 15000, cancellationToken);

            var planta = await uow.Comunes.Plantas.Query()
                .FirstOrDefaultAsync(p => p.Code == plantaCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Planta {plantaCode} no encontrada.");

            var numbers = new List<string>();
            var current = planta.NumeroLet;

            while (numbers.Count < count)
            {
                current = IncrementNumber(current);
                var n = current;

                // Salvaguarda ante numeración desfasada respecto a letras ya registradas.
                if (await uow.Facturacion.Transacciones.LetrasVenta.Query()
                        .AnyAsync(x => x.PlantaCode == plantaCode && x.Number == n, cancellationToken))
                {
                    continue;
                }

                numbers.Add(current);
            }

            planta.NumeroLet = current;

            return numbers;
        }

        public static async Task<LetraVenta> GetTrackedAsync(
            IUnitOfWork uow, string plantaCode, string number, CancellationToken cancellationToken)
            => await uow.Facturacion.Transacciones.LetrasVenta.Query()
                   .FirstOrDefaultAsync(l => l.PlantaCode == plantaCode && l.Number == number, cancellationToken)
               ?? throw new KeyNotFoundException($"Letra {FormatKey(plantaCode, number)} no encontrada.");

        public static void EnsureRowVersion(LetraVenta letra, byte[] rowVersion)
        {
            if (!letra.RowVersion.SequenceEqual(rowVersion))
            {
                throw new InvalidOperationException(
                    "La letra fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }
        }

        public static Task EnsurePeriodOpenAsync(IUnitOfWork uow, DateTime date, CancellationToken cancellationToken)
            => PeriodoContableRules.EnsureOpenAsync(uow, date.Year.ToString("0000"), date.Month.ToString("00"), cancellationToken);
    }
}
