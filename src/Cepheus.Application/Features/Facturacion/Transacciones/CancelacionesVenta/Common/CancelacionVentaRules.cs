using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common
{
    /// <summary>Códigos de tipo de pago con comportamiento propio en la cancelación (legacy Codigo_tpa).</summary>
    public static class CancelacionTipoPago
    {
        public const string Efectivo = "01";
        public const string Cheque = "02";
        public const string Letras = "03";
        public const string Canje = "04";
        public const string ChequeDiferido = "07";
        public const string LetrasVarias = "11";

        /// <summary>El pago deja un cheque registrado (RCheVentas).</summary>
        public static bool GeneraCheque(string code) => code is Cheque or ChequeDiferido;

        /// <summary>El pago deja una letra registrada (RLetVentas).</summary>
        public static bool GeneraLetra(string code) => code == Letras;
    }

    public static class CancelacionVentaRules
    {
        public const int AsientoLength = 13;

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static string ItemNumber(int n) => n.ToString("000");

        public static bool IsAsiento(string? value)
            => !string.IsNullOrWhiteSpace(value) && value.Trim().Length == AsientoLength;

        public static bool HasAsiento(CancelacionVenta c) => IsAsiento(c.ProvisionEntryNumber);

        /// <summary>Legacy ue_set_btn_mod_dw: con asiento generado la cancelación ya no se modifica.</summary>
        public static bool CanEdit(CancelacionVenta c) => c.Status == EstadoCancelacion.Pendiente && !HasAsiento(c);

        public static void EnsureEditable(CancelacionVenta c)
        {
            if (HasAsiento(c))
            {
                throw new InvalidOperationException("El asiento de cancelación ya fue generado; anúlelo para modificar.");
            }

            if (c.Status != EstadoCancelacion.Pendiente)
            {
                throw new InvalidOperationException($"La cancelación {c.Code} está {c.Status}; solo se modifica en estado Pendiente.");
            }
        }

        /// <summary>
        /// Agregar, retirar o cuadrar documentos: la cancelación debe ser editable y, en Letras Varias (11), los
        /// documentos son los que se refinanciaron en las letras y no se cambian (se anula el asiento para deshacer).
        /// </summary>
        public static void EnsureDocumentsEditable(CancelacionVenta c)
        {
            EnsureEditable(c);

            if (c.TipoPagoCode == CancelacionTipoPago.LetrasVarias)
            {
                throw new InvalidOperationException(
                    "En Letras Varias los documentos no se modifican: están refinanciados en las letras generadas.");
            }
        }

        /// <summary>Código de 13 caracteres de un asiento: año + mes + tipo + número.</summary>
        public static string AsientoCode(Cepheus.Domain.Contabilidad.Transacciones.AsientoContable a)
            => a.Anio + a.Mes + a.TipoAsientoCode + a.Numero;

        public static async Task<CancelacionVenta> GetTrackedAsync(
            IUnitOfWork uow, string code, CancellationToken cancellationToken)
            => await uow.Facturacion.Transacciones.CancelacionesVenta.Query()
                   .Include(c => c.Details)
                   .FirstOrDefaultAsync(c => c.Code == code, cancellationToken)
               ?? throw new KeyNotFoundException($"Cancelación {code} no encontrada.");

        public static void EnsureRowVersion(CancelacionVenta c, byte[] rowVersion)
        {
            if (!c.RowVersion.SequenceEqual(rowVersion))
            {
                throw new InvalidOperationException(
                    "La cancelación fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }
        }

        public static Task EnsurePeriodOpenAsync(IUnitOfWork uow, DateTime date, CancellationToken cancellationToken)
            => PeriodoContableRules.EnsureOpenAsync(uow, date.Year.ToString("0000"), date.Month.ToString("00"), cancellationToken);

        /// <summary>Porcentaje de tolerancia de la cancelación (legacy gd_porcancelacion = ControlVentas.QuotaPercentage).</summary>
        public static async Task<decimal> GetTolerancePercentAsync(IUnitOfWork uow, CancellationToken cancellationToken)
            => await uow.Comunes.ControlesVentas.Query().AsNoTracking()
                .Select(c => c.QuotaPercentage)
                .FirstOrDefaultAsync(cancellationToken);

        /// <summary>
        /// Legacy ue_set_btn_grb_data: lo aplicado a documentos no puede superar el importe de la cancelación más el
        /// porcentaje de tolerancia. No aplica al canje (tipo de pago 04), que no mueve dinero.
        /// </summary>
        public static async Task EnsureWithinTotalAsync(IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            if (c.TipoPagoCode == CancelacionTipoPago.Canje)
            {
                return;
            }

            var pct = await GetTolerancePercentAsync(uow, cancellationToken);

            var used = c.Details.Sum(l => CobranzaRules.ToCancelacionCurrency(l.PaidAmount, l.PaidSoles, l.DocumentCurrencyCode, c));

            if (c.TotalAmount + c.TotalAmount * pct / 100m < used)
            {
                throw new InvalidOperationException("Monto mayor al disponible: los documentos superan el importe de la cancelación.");
            }
        }

        /// <summary>
        /// Recalcula totales de la cancelación y saldos de sus documentos. Cheque diferido (07): solo la cancelación.
        /// Las líneas ya deben estar guardadas.
        /// </summary>
        public static Task RecalculateAsync(
            IUnitOfWork uow, CancelacionVenta c, bool apply, CancellationToken cancellationToken)
            => CobranzaDocumentoService.ActualizarCancelacionAsync(
                uow, c.Code, apply, cancellationToken, updateDocuments: c.TipoPagoCode != CancelacionTipoPago.ChequeDiferido);
    }
}
