using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Transacciones;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common
{
    /// <summary>
    /// Documento que se cancela. <c>Amount</c> es positivo y va en la moneda de la cancelación (si no se indica se
    /// aplica todo el saldo del documento, como al "jalar documentos" del legacy); las notas de crédito restan solas.
    /// </summary>
    public class CancelacionVentaDocumentoInput
    {
        public string TypeCode { get; set; } = default!;

        /// <summary>Obligatoria para 07, 08 y 10 (el número se repite entre plantas).</summary>
        public string? PlantaCode { get; set; }
        public string Number { get; set; } = default!;

        /// <summary>Obligatorio solo para cheques (11).</summary>
        public string? BancoCode { get; set; }
        public decimal? Amount { get; set; }

        /// <summary>Legacy IND_DETRACCION.</summary>
        public bool IsDetraction { get; set; }
    }

    /// <summary>
    /// Agrega documentos a una cancelación (legacy: "jalar documentos" dw_cre_det_documentos y "por documento"
    /// cr_can_por_documento). No guarda: quien llama hace el SaveChanges y luego recalcula saldos.
    /// </summary>
    public static class CancelacionVentaDocumentosService
    {
        public static async Task<List<CancelacionVentaDetalle>> AddAsync(
            IUnitOfWork uow, CancelacionVenta c, IReadOnlyList<CancelacionVentaDocumentoInput> inputs,
            CancellationToken cancellationToken)
        {
            var added = new List<CancelacionVentaDetalle>();
            var seen = new HashSet<string>(c.Details.Select(d => Key(d.TypeCode, d.PlantaCode, d.DocumentNumber, d.DocumentBancoCode)));
            var nextItem = c.Details.Select(d => int.TryParse(d.Item, out var n) ? n : 0).DefaultIfEmpty(0).Max();

            foreach (var input in inputs)
            {
                var type = input.TypeCode.Trim();
                var number = input.Number.Trim();
                var planta = string.IsNullOrWhiteSpace(input.PlantaCode) ? null : LetraVentaRules.Normalize(input.PlantaCode);
                var banco = string.IsNullOrWhiteSpace(input.BancoCode) ? null : input.BancoCode.Trim().ToUpperInvariant();

                if (type is CobranzaTipoDocumento.NotaCredito or CobranzaTipoDocumento.NotaDebito or CobranzaTipoDocumento.Letra
                    && planta is null)
                {
                    throw new InvalidOperationException($"Indique la planta del documento {type} {number}.");
                }

                if (type == CobranzaTipoDocumento.Cheque && banco is null)
                {
                    throw new InvalidOperationException($"Indique el banco del cheque {number}.");
                }

                if (!seen.Add(Key(type, planta, number, banco)))
                {
                    throw new InvalidOperationException("Documento ya se encuentra registrado: " + type + " " + number + ".");
                }

                var doc = await CobranzaDocumentoService.LoadAsync(uow, type, planta, number, c.ClienteCode, banco, cancellationToken)
                    ?? throw new KeyNotFoundException($"Documento {type} {number} no encontrado.");

                if (doc.ClienteCode != c.ClienteCode)
                {
                    throw new InvalidOperationException($"El documento {type} {number} no pertenece al cliente {c.ClienteCode}.");
                }

                if (!doc.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"El documento {type} {number} no puede ser cancelado por esta opción: debe estar por cobrar (Facturado), emitido, con asiento de provisión y sin estar incluido en una letra.");
                }

                // Importe en la moneda de la cancelación (por defecto, todo el saldo del documento).
                var maxAmount = doc.CurrencyCode == c.MonedaCode
                    ? doc.Pending
                    : c.MonedaCode == CobranzaRules.Moneda
                        ? DocumentoVentaRules.Round2(doc.Pending * doc.ExchangeRate)
                        : DocumentoVentaRules.Round2(doc.Pending / c.ExchangeRate);

                var amount = input.Amount ?? maxAmount;

                var (paid, paidSoles) = CobranzaRules.ToLineAmounts(amount, c.MonedaCode, c.ExchangeRate, doc.CurrencyCode, doc.ExchangeRate);

                if (input.Amount is null)
                {
                    paid = doc.Pending; // todo el saldo, sin diferencias de redondeo
                }

                // Legacy dw_3 itemchanged: la cantidad no puede ser mayor al saldo del documento.
                if (paid <= 0 || paid > doc.Pending + CobranzaDocumentoService.Tolerance)
                {
                    throw new InvalidOperationException(
                        $"El importe del documento {type} {number} no puede ser mayor a su saldo ({doc.Pending:N2} {doc.CurrencyCode}).");
                }

                var sign = type == CobranzaTipoDocumento.NotaCredito ? -1m : 1m;

                // Legacy: diferencia de cambio solo cuando cancelación y documento son en moneda extranjera y las
                // fechas difieren: importe al tipo de cambio de la cancelación - importe al del documento.
                var difference = 0m;
                if (c.MonedaCode != CobranzaRules.Moneda && doc.CurrencyCode == c.MonedaCode && doc.IssueDate.Date != c.Date.Date)
                {
                    difference = DocumentoVentaRules.Round2(paid * c.ExchangeRate) - DocumentoVentaRules.Round2(paid * doc.ExchangeRate);
                }

                nextItem++;

                var line = new CancelacionVentaDetalle
                {
                    CancelacionCode = c.Code,
                    Item = CancelacionVentaRules.ItemNumber(nextItem),
                    PlantaCode = doc.PlantaCode ?? c.PlantaCode,
                    ClienteCode = c.ClienteCode,
                    CancelacionDate = c.Date,
                    TypeCode = doc.TypeCode,
                    DocumentNumber = doc.Number,
                    DocumentBancoCode = doc.BancoCode,
                    DocumentDate = doc.IssueDate,
                    DocumentCurrencyCode = doc.CurrencyCode,
                    DocumentExchangeRate = doc.ExchangeRate,
                    NetAmount = sign * doc.Total,
                    PaidAmount = sign * paid,
                    PaidSoles = sign * paidSoles,
                    DifferenceSoles = sign * difference,
                    ProvisionEntryNumber = doc.ProvisionEntry,
                    ObraCode = doc.ObraCode,
                    IsDoubtful = false,
                    DoubtfulExchangeRate = 0m,
                    IsDetraction = input.IsDetraction
                };

                c.Details.Add(line);
                added.Add(line);
            }

            return added;
        }

        private static string Key(string type, string? planta, string number, string? banco)
            => $"{type}|{(type is CobranzaTipoDocumento.NotaCredito or CobranzaTipoDocumento.NotaDebito or CobranzaTipoDocumento.Letra ? planta : string.Empty)}|{number}|{banco}";
    }
}
