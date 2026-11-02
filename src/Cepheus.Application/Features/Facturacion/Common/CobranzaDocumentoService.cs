using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Common
{
    /// <summary>Códigos de tipo de documento que maneja la cobranza (legacy Codigo_tdo).</summary>
    public static class CobranzaTipoDocumento
    {
        public const string Factura = "01";
        public const string Boleta = "03";
        public const string NotaCredito = "07";
        public const string NotaDebito = "08";
        public const string Letra = "10";
        public const string Cheque = "11";

        public static readonly string[] Todos = { Factura, Boleta, NotaCredito, NotaDebito, Letra, Cheque };
    }

    /// <summary>Un documento por cobrar visto de forma uniforme (factura, boleta, NC, ND, letra o cheque).</summary>
    public sealed class CobranzaDocumento
    {
        public string TypeCode { get; init; } = default!;
        public string? PlantaCode { get; init; }
        public string Number { get; init; } = default!;
        public string ClienteCode { get; init; } = default!;
        public string? BancoCode { get; init; }
        public string CurrencyCode { get; init; } = default!;
        public decimal ExchangeRate { get; init; }
        public DateTime IssueDate { get; init; }
        public decimal Total { get; init; }
        public decimal Pending { get; init; }
        public int Status { get; init; }
        public bool IsReboteCheque { get; init; }

        public string? ObraCode { get; init; }
        public string? ProvisionEntry { get; init; }
        public bool IsPrinted { get; init; }
        public string? CashDocument { get; init; }

        /// <summary>
        /// El documento puede incluirse en una cancelación / letras: Facturado (por cobrar), emitido, con asiento de
        /// provisión y no incluido ya en una letra (legacy cr_consulta_documentos).
        /// </summary>
        public bool IsAvailable => Status == 2 && IsPrinted && !string.IsNullOrWhiteSpace(ProvisionEntry)
                                   && string.IsNullOrWhiteSpace(CashDocument);

        /// <summary>Marca el documento como incluido en una letra (legacy Documento_Caj). Solo 01, 03 y 07.</summary>
        internal Action<string?>? SetCashDocument { get; init; }

        /// <summary>Escribe saldo, estado y fecha de cancelación en la entidad (no guarda).</summary>
        internal Action<decimal, int, DateTime?> Apply { get; init; } = default!;
    }

    /// <summary>
    /// Motor de saldos de la cobranza. Reemplaza a los SPs del legacy ad_actualizadocumentos y
    /// ad_actualizacancelaciones, que recalculaban el saldo y el estado de cada documento a partir de lo
    /// aplicado en las cancelaciones (DCanDocumentos).
    ///
    /// Lo usan Letras, Cheques y Cancelaciones. Lee los importes aplicados de la BASE DE DATOS, por eso quien lo
    /// llama debe haber guardado (SaveChanges) las líneas de cancelación antes. No guarda: el SaveChanges final
    /// lo hace quien llama, dentro de su transacción.
    ///
    /// Reglas del legacy que conserva:
    ///   - Saldo = total del documento - suma del valor absoluto de lo aplicado (las NC se guardan en negativo).
    ///   - Si el saldo es menor a 0.03 (tolerancia), se está "aplicando" (accion '1') y hay algo aplicado, el
    ///     documento queda Cancelado con saldo 0.
    ///   - Si el saldo es mayor a 0 y el documento estaba Cancelado, vuelve a Facturado (por cobrar). Un cheque
    ///     solo vuelve a Rebotado si tiene motivo de rebote: un cheque cobrado normal conserva su estado.
    ///   - Fecha de cancelación: la fecha dada, o la de la última cancelación que lo aplicó.
    ///   - Los cheques mixtos (legacy DCheMixtos, tipo 'V') se suman al aplicado: pagan documentos de cliente desde
    ///     Compras/Tesorería. Se guardan en soles; en documentos USD se convierten con el tipo de cambio del documento.
    /// </summary>
    public static class CobranzaDocumentoService
    {
        public const decimal Tolerance = 0.03m;

        private const int Facturado = 2;
        private const int Cancelado = 3;

        public static async Task<CobranzaDocumento?> LoadAsync(
            IUnitOfWork uow, string typeCode, string? plantaCode, string number, string clienteCode, string? bancoCode,
            CancellationToken cancellationToken)
        {
            var t = uow.Facturacion.Transacciones;

            switch (typeCode)
            {
                case CobranzaTipoDocumento.Factura:
                case CobranzaTipoDocumento.Boleta:
                {
                    var c = await t.ComprobantesVenta.Query()
                        .FirstOrDefaultAsync(x => x.TypeCode == typeCode && x.Number == number, cancellationToken);
                    if (c is null) return null;

                    return new CobranzaDocumento
                    {
                        TypeCode = typeCode, PlantaCode = c.PlantCode, Number = c.Number, ClienteCode = c.ClienteCode,
                        CurrencyCode = c.CurrencyCode, ExchangeRate = c.ExchangeRate, IssueDate = c.IssueDate,
                        Total = c.TotalAmount, Pending = c.PendingAmount, Status = (int)c.Status,
                        ObraCode = c.ObraCode, ProvisionEntry = c.ProvisionEntry, IsPrinted = c.IsPrinted, CashDocument = c.CashDocument,
                        SetCashDocument = v => c.CashDocument = v,
                        Apply = (pending, status, settled) =>
                        {
                            c.PendingAmount = pending;
                            c.Status = (EstadoComprobanteVenta)status;
                            c.SettledAt = settled;
                            c.CashDocument = null; // legacy: Documento_Caj = null
                        }
                    };
                }

                case CobranzaTipoDocumento.NotaCredito:
                {
                    var n = await t.NotasCreditoVenta.Query()
                        .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Number == number, cancellationToken);
                    if (n is null) return null;

                    return new CobranzaDocumento
                    {
                        TypeCode = typeCode, PlantaCode = n.PlantaCode, Number = n.Number, ClienteCode = n.ClienteCode,
                        CurrencyCode = n.MonedaCode, ExchangeRate = n.ExchangeRate, IssueDate = n.IssueDate,
                        Total = n.TotalAmount, Pending = n.PendingAmount, Status = (int)n.Status,
                        ObraCode = n.ObraCode, ProvisionEntry = n.ProvisionEntryNumber, IsPrinted = n.IsPrinted, CashDocument = n.CashDocument,
                        SetCashDocument = v => n.CashDocument = v,
                        Apply = (pending, status, settled) =>
                        {
                            n.PendingAmount = pending;
                            n.Status = (EstadoNotaCredito)status;
                            n.SettledAt = settled;
                            n.CashDocument = null;
                        }
                    };
                }

                case CobranzaTipoDocumento.NotaDebito:
                {
                    var n = await t.NotasDebitoVenta.Query()
                        .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Number == number, cancellationToken);
                    if (n is null) return null;

                    return new CobranzaDocumento
                    {
                        TypeCode = typeCode, PlantaCode = n.PlantaCode, Number = n.Number, ClienteCode = n.ClienteCode,
                        CurrencyCode = n.MonedaCode, ExchangeRate = n.ExchangeRate, IssueDate = n.IssueDate,
                        Total = n.TotalAmount, Pending = n.PendingAmount, Status = (int)n.Status,
                        ObraCode = n.ObraCode, ProvisionEntry = n.ProvisionEntryNumber, IsPrinted = n.IsPrinted,
                        Apply = (pending, status, settled) =>
                        {
                            n.PendingAmount = pending;
                            n.Status = (EstadoNotaDebito)status;
                            n.SettledAt = settled;
                        }
                    };
                }

                case CobranzaTipoDocumento.Letra:
                {
                    var l = await t.LetrasVenta.Query()
                        .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Number == number, cancellationToken);
                    if (l is null) return null;

                    return new CobranzaDocumento
                    {
                        TypeCode = typeCode, PlantaCode = l.PlantaCode, Number = l.Number, ClienteCode = l.ClienteCode,
                        CurrencyCode = l.MonedaCode, ExchangeRate = l.ExchangeRate, IssueDate = l.IssueDate,
                        Total = l.TotalAmount, Pending = l.PendingAmount, Status = (int)l.Status,
                        ObraCode = "001", ProvisionEntry = l.ProvisionEntryNumber, IsPrinted = l.IsPrinted,
                        Apply = (pending, status, settled) =>
                        {
                            l.PendingAmount = pending;
                            l.Status = (EstadoLetra)status;
                            l.SettledAt = settled;
                        }
                    };
                }

                case CobranzaTipoDocumento.Cheque:
                {
                    var q = t.ChequesVenta.Query().Where(x => x.Number == number && x.ClienteCode == clienteCode);
                    if (!string.IsNullOrWhiteSpace(bancoCode))
                    {
                        q = q.Where(x => x.BancoCode == bancoCode);
                    }

                    var found = await q.Take(2).ToListAsync(cancellationToken);
                    if (found.Count == 0) return null;
                    if (found.Count > 1)
                    {
                        throw new InvalidOperationException(
                            $"El cheque {number} existe en más de un banco para el cliente {clienteCode}: indique el banco.");
                    }

                    var c = found[0];

                    return new CobranzaDocumento
                    {
                        TypeCode = typeCode, PlantaCode = c.PlantaCode, Number = c.Number, ClienteCode = c.ClienteCode,
                        BancoCode = c.BancoCode, CurrencyCode = c.MonedaCode, ExchangeRate = c.ExchangeRate,
                        IssueDate = c.IssueDate, Total = c.TotalAmount, Pending = c.PendingAmount, Status = (int)c.Status,
                        IsReboteCheque = !string.IsNullOrWhiteSpace(c.MotivoReboteCode),
                        ObraCode = "001", ProvisionEntry = c.ProvisionEntryNumber, IsPrinted = true,
                        Apply = (pending, status, settled) =>
                        {
                            c.PendingAmount = pending;
                            c.Status = (EstadoCheque)status;
                            c.SettledAt = settled;
                        }
                    };
                }

                default:
                    throw new InvalidOperationException($"Tipo de documento {typeCode} no soportado en cobranzas.");
            }
        }

        /// <summary>Total aplicado al documento (valor absoluto), sin contar cancelaciones anuladas.</summary>
        public static async Task<decimal> GetAppliedAmountAsync(
            IUnitOfWork uow, CobranzaDocumento doc, CancellationToken cancellationToken)
        {
            var cancelaciones = await AppliedLines(uow, doc)
                .SumAsync(d => (decimal?)Math.Abs(d.PaidAmount), cancellationToken) ?? 0m;

            var mixtos = await uow.Facturacion.Transacciones.ChequesMixtosDocumentos.Query().AsNoTracking()
                .Where(m => m.ChequeType == "V" && m.TypeCode == doc.TypeCode
                            && m.DocumentNumber == doc.Number && m.ClienteCode == doc.ClienteCode)
                .Select(m => new { m.PaidSoles, m.DocumentExchangeRate })
                .ToListAsync(cancellationToken);

            decimal mixtosAmount = 0m;
            foreach (var m in mixtos)
            {
                var soles = Math.Abs(m.PaidSoles);
                if (doc.CurrencyCode == "PEN")
                {
                    mixtosAmount += soles;
                }
                else
                {
                    var rate = m.DocumentExchangeRate > 0 ? m.DocumentExchangeRate : doc.ExchangeRate;
                    if (rate > 0) mixtosAmount += Math.Round(soles / rate, 2);
                }
            }

            return cancelaciones + mixtosAmount;
        }

        private static IQueryable<CancelacionVentaDetalle> AppliedLines(IUnitOfWork uow, CobranzaDocumento doc)
        {
            var q = uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Query().AsNoTracking()
                .Where(d => d.TypeCode == doc.TypeCode
                            && d.DocumentNumber == doc.Number
                            && d.ClienteCode == doc.ClienteCode
                            && d.Cancelacion.Status != EstadoCancelacion.Anulado);

            if (doc.TypeCode is CobranzaTipoDocumento.NotaCredito or CobranzaTipoDocumento.NotaDebito
                or CobranzaTipoDocumento.Letra)
            {
                q = q.Where(d => d.PlantaCode == doc.PlantaCode);
            }
            else if (doc.TypeCode == CobranzaTipoDocumento.Cheque && !string.IsNullOrWhiteSpace(doc.BancoCode))
            {
                q = q.Where(d => d.DocumentBancoCode == doc.BancoCode);
            }

            return q;
        }

        /// <summary>
        /// Recalcula saldo, estado y fecha de cancelación del documento (legacy ad_actualizadocumentos).
        /// <paramref name="apply"/> equivale a accion '1' (se está aplicando un pago); false es accion '0'.
        /// Si el documento no existe no hace nada (como el legacy).
        /// </summary>
        public static async Task ActualizarAsync(
            IUnitOfWork uow, string typeCode, string? plantaCode, string number, string clienteCode, string? bancoCode,
            bool apply, DateTime? date, CancellationToken cancellationToken)
        {
            var doc = await LoadAsync(uow, typeCode, plantaCode, number, clienteCode, bancoCode, cancellationToken);
            if (doc is null)
            {
                return;
            }

            var paid = await GetAppliedAmountAsync(uow, doc, cancellationToken);

            var pending = doc.Total - paid;
            var status = doc.Status;

            if (pending < Tolerance && apply && paid > 0)
            {
                status = Cancelado;
                pending = 0m;
            }

            if (pending > 0 && status == Cancelado)
            {
                if (typeCode != CobranzaTipoDocumento.Cheque || doc.IsReboteCheque)
                {
                    status = Facturado;
                }
            }

            DateTime? settled = null;

            if (status == Cancelado)
            {
                settled = date is { } d && d.Year >= 1997 ? d : await LastCancelacionDateAsync(uow, doc, cancellationToken);
            }

            doc.Apply(pending, status, settled);
        }

        private static async Task<DateTime?> LastCancelacionDateAsync(
            IUnitOfWork uow, CobranzaDocumento doc, CancellationToken cancellationToken)
        {
            var fromLines = await AppliedLines(uow, doc)
                .OrderByDescending(d => d.CancelacionDate)
                .Select(d => (DateTime?)d.CancelacionDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (doc.TypeCode != CobranzaTipoDocumento.Cheque)
            {
                return fromLines;
            }

            // El cheque con que se pagó otra cancelación (tipo de pago 02 y mismo número).
            var asPayment = await uow.Facturacion.Transacciones.CancelacionesVenta.Query().AsNoTracking()
                .Where(c => c.TipoPagoCode == "02" && c.PaymentNumber == doc.Number && c.ClienteCode == doc.ClienteCode
                            && c.Status != EstadoCancelacion.Anulado)
                .OrderByDescending(c => c.Date)
                .Select(c => (DateTime?)c.Date)
                .FirstOrDefaultAsync(cancellationToken);

            return fromLines is null ? asPayment : asPayment is null ? fromLines : fromLines > asPayment ? fromLines : asPayment;
        }

        /// <summary>
        /// Recalcula los totales de la cancelación y actualiza todos sus documentos
        /// (legacy ad_actualizacancelaciones; con <paramref name="updateDocuments"/> en false, COM_SP_ACTUALIZA_CAN_CHEDIF). <paramref name="apply"/> equivale a accion '1': si ya no queda saldo
        /// disponible la cancelación pasa a Cancelado. Las líneas deben estar guardadas.
        /// </summary>
        public static async Task ActualizarCancelacionAsync(
            IUnitOfWork uow, string cancelacionCode, bool apply, CancellationToken cancellationToken,
            bool updateDocuments = true)
        {
            var cancelacion = await uow.Facturacion.Transacciones.CancelacionesVenta.Query()
                .Include(c => c.Details)
                .FirstOrDefaultAsync(c => c.Code == cancelacionCode, cancellationToken)
                ?? throw new KeyNotFoundException($"Cancelación {cancelacionCode} no encontrada.");

            var used = 0m;
            var diff = 0m;

            foreach (var line in cancelacion.Details)
            {
                used += CobranzaRules.ToCancelacionCurrency(line.PaidAmount, line.PaidSoles, line.DocumentCurrencyCode, cancelacion);
                diff += CobranzaRules.SolesToCancelacionCurrency(line.DifferenceSoles, cancelacion);
            }

            cancelacion.UsedAmount = DocumentoVentaRules.Round2(used);
            cancelacion.DifferenceAmount = DocumentoVentaRules.Round2(diff);
            cancelacion.AvailableAmount = DocumentoVentaRules.Round2(cancelacion.TotalAmount - (cancelacion.UsedAmount + cancelacion.DifferenceAmount));

            if (apply && cancelacion.AvailableAmount <= 0 && cancelacion.Status != EstadoCancelacion.Anulado)
            {
                cancelacion.Status = EstadoCancelacion.Cancelado;
            }

            // Legacy COM_SP_ACTUALIZA_CAN_CHEDIF (cheque diferido, tipo de pago 07): solo recalcula la cancelación,
            // los documentos se actualizan recién al generar el asiento.
            if (!updateDocuments)
            {
                return;
            }

            // El legacy siempre actualiza los documentos con accion '1'.
            foreach (var line in cancelacion.Details.ToList())
            {
                await ActualizarAsync(
                    uow, line.TypeCode, line.PlantaCode, line.DocumentNumber, line.ClienteCode, line.DocumentBancoCode,
                    apply: true, cancelacion.Date, cancellationToken);
            }
        }
    }

    /// <summary>Conversiones de importes de la cobranza.</summary>
    public static class CobranzaRules
    {
        public const string Moneda = DocumentoVentaRules.MonedaBase;

        /// <summary>
        /// Importes de una línea de cancelación a partir de un monto expresado en la moneda de la cancelación:
        /// lo aplicado en la moneda del documento y su equivalente en soles. Solo soporta soles y una moneda extranjera.
        /// </summary>
        public static (decimal Paid, decimal PaidSoles) ToLineAmounts(
            decimal amount, string cancelacionCurrency, decimal cancelacionRate, string documentCurrency, decimal documentRate)
        {
            if (cancelacionCurrency == documentCurrency)
            {
                return (amount, documentCurrency == Moneda ? amount : DocumentoVentaRules.Round2(amount * documentRate));
            }

            if (cancelacionCurrency == Moneda)
            {
                // Se paga en soles un documento en moneda extranjera.
                return (DocumentoVentaRules.Round2(amount / documentRate), amount);
            }

            if (documentCurrency == Moneda)
            {
                // Se paga en moneda extranjera un documento en soles.
                var soles = DocumentoVentaRules.Round2(amount * cancelacionRate);
                return (soles, soles);
            }

            throw new InvalidOperationException(
                $"No se puede aplicar un pago en {cancelacionCurrency} a un documento en {documentCurrency}.");
        }

        /// <summary>Lo aplicado por una línea expresado en la moneda de la cancelación.</summary>
        public static decimal ToCancelacionCurrency(
            decimal paid, decimal paidSoles, string documentCurrency, CancelacionVenta cancelacion)
        {
            if (cancelacion.MonedaCode == documentCurrency)
            {
                return paid;
            }

            return SolesToCancelacionCurrency(paidSoles, cancelacion);
        }

        public static decimal SolesToCancelacionCurrency(decimal soles, CancelacionVenta cancelacion)
            => cancelacion.MonedaCode == Moneda || cancelacion.ExchangeRate <= 0
                ? soles
                : DocumentoVentaRules.Round2(soles / cancelacion.ExchangeRate);
    }
}
