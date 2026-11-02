using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GenerarLetrasVarias
{
    public class GenerarLetrasVariasCommandHandler
        : IRequestHandler<GenerarLetrasVariasCommand, GenerarLetrasVariasResponse>
    {
        private const string TipoPagoLetrasVarias = "11";
        private const int DaysBetweenInstallments = 30;

        private readonly IUnitOfWork _uow;

        public GenerarLetrasVariasCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private sealed class Line
        {
            public CobranzaDocumento Doc { get; init; } = default!;
            public decimal Sign { get; init; }
            public decimal Amount { get; init; }       // en la moneda de las letras, con signo
            public decimal Paid { get; init; }         // en la moneda del documento, con signo
            public decimal PaidSoles { get; init; }    // con signo
        }

        public async Task<GenerarLetrasVariasResponse> Handle(
            GenerarLetrasVariasCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = LetraVentaRules.Normalize(request.PlantaCode);
            var clienteCode = request.ClienteCode.Trim();
            var cobradorCode = request.CobradorCode.Trim();
            var moneda = request.MonedaCode.Trim().ToUpperInvariant();
            var date = request.Date.Date;

            // Legacy: la fecha no puede ser mayor a la fecha de proceso ni caer en un mes cerrado.
            if (date > DateTime.Today)
            {
                throw new InvalidOperationException("La fecha no puede ser mayor a la fecha de proceso.");
            }

            await PeriodoContableRules.EnsureOpenAsync(_uow, date.Year.ToString("0000"), date.Month.ToString("00"), cancellationToken);

            if (!await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == plantaCode, cancellationToken))
                throw new KeyNotFoundException($"Planta {plantaCode} no encontrada.");
            if (!await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(c => c.Code == clienteCode, cancellationToken))
                throw new KeyNotFoundException($"Cliente {clienteCode} no encontrado.");
            if (!await _uow.Facturacion.Maestros.Cobradores.Query().AnyAsync(c => c.Code == cobradorCode && c.IsActive, cancellationToken))
                throw new InvalidOperationException($"El cobrador {cobradorCode} no existe o está inactivo.");
            if (!await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == moneda && m.IsActive, cancellationToken))
                throw new InvalidOperationException($"La moneda {moneda} no existe o está inactiva.");

            var rate = await DocumentoVentaRules.ResolveExchangeRateAsync(_uow, moneda, date, cancellationToken);

            GenerarLetrasVariasResponse response;

            if (request.PreviewOnly)
            {
                response = await BuildAsync(request, plantaCode, clienteCode, cobradorCode, moneda, date, rate, save: false, cancellationToken);
                _uow.ClearTracking();
                return response;
            }

            if (!await _uow.Facturacion.Catalogos.TiposPago.Query().AnyAsync(t => t.Code == TipoPagoLetrasVarias, cancellationToken))
            {
                throw new InvalidOperationException(
                    "No existe el tipo de pago 11 (Letras Varias). Regístrelo en el catálogo de tipos de pago.");
            }

            try
            {
                response = await _uow.ExecuteInTransactionAsync(
                    ct => BuildAsync(request, plantaCode, clienteCode, cobradorCode, moneda, date, rate, save: true, ct),
                    System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "Alguno de los documentos fue modificado por otro proceso. Vuelva a consultar los pendientes e intente nuevamente.");
            }

            _uow.ClearTracking();

            return response;
        }

        private async Task<GenerarLetrasVariasResponse> BuildAsync(
            GenerarLetrasVariasCommand request, string plantaCode, string clienteCode, string cobradorCode,
            string moneda, DateTime date, decimal rate, bool save, CancellationToken ct)
        {
            // ---- Documentos y total
            var lines = new List<Line>();
            var seen = new HashSet<string>();

            foreach (var input in request.Documents)
            {
                var type = input.TypeCode.Trim();
                var number = input.Number.Trim();
                var docPlanta = string.IsNullOrWhiteSpace(input.PlantaCode) ? null : LetraVentaRules.Normalize(input.PlantaCode);
                var banco = string.IsNullOrWhiteSpace(input.BancoCode) ? null : input.BancoCode.Trim().ToUpperInvariant();

                if (type is CobranzaTipoDocumento.NotaCredito or CobranzaTipoDocumento.NotaDebito or CobranzaTipoDocumento.Letra
                    && docPlanta is null)
                {
                    throw new InvalidOperationException($"Indique la planta del documento {type} {number}.");
                }

                if (!seen.Add($"{type}|{docPlanta}|{number}|{banco}"))
                {
                    throw new InvalidOperationException($"El documento {type} {number} está repetido.");
                }

                var doc = await CobranzaDocumentoService.LoadAsync(_uow, type, docPlanta, number, clienteCode, banco, ct)
                    ?? throw new KeyNotFoundException($"Documento {type} {number} no encontrado.");

                if (doc.ClienteCode != clienteCode)
                {
                    throw new InvalidOperationException($"El documento {type} {number} no pertenece al cliente {clienteCode}.");
                }

                if (!doc.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"El documento {type} {number} no está disponible: debe estar por cobrar (Facturado), emitido, con asiento y sin estar incluido en otras letras.");
                }

                var sign = type == CobranzaTipoDocumento.NotaCredito ? -1m : 1m;

                // Saldo del documento expresado en la moneda de las letras.
                var maxAmount = doc.CurrencyCode == moneda
                    ? doc.Pending
                    : moneda == CobranzaRules.Moneda
                        ? DocumentoVentaRules.Round2(doc.Pending * doc.ExchangeRate)
                        : DocumentoVentaRules.Round2(doc.Pending / rate);

                var amount = input.Amount ?? maxAmount;

                var (paid, paidSoles) = CobranzaRules.ToLineAmounts(amount, moneda, rate, doc.CurrencyCode, doc.ExchangeRate);

                if (input.Amount is null)
                {
                    paid = doc.Pending; // todo el saldo, sin diferencias de redondeo
                }

                if (paid > doc.Pending + CobranzaDocumentoService.Tolerance || paid <= 0)
                {
                    throw new InvalidOperationException(
                        $"El importe del documento {type} {number} excede su saldo ({doc.Pending:N2} {doc.CurrencyCode}).");
                }

                lines.Add(new Line { Doc = doc, Sign = sign, Amount = sign * amount, Paid = sign * paid, PaidSoles = sign * paidSoles });
            }

            var totalDocuments = DocumentoVentaRules.Round2(lines.Sum(l => l.Amount));
            var initial = DocumentoVentaRules.Round2(request.InitialAmount);

            if (totalDocuments <= 0)
            {
                throw new InvalidOperationException("No se puede cancelar con monto 0: el total de los documentos debe ser mayor a cero.");
            }

            if (initial >= totalDocuments)
            {
                throw new InvalidOperationException("La cuota inicial debe ser menor al total de los documentos.");
            }

            // ---- Cuotas
            var financed = totalDocuments - initial;
            var count = request.InstallmentCount;
            var interestTotal = request.InstallmentType == "I" ? DocumentoVentaRules.Round2(request.Interest) : 0m;
            var baseAmount = DocumentoVentaRules.Round2(financed / count);
            var baseInterest = interestTotal > 0 ? DocumentoVentaRules.Round2(interestTotal / count) : 0m;

            var cuotas = new List<LetraVariaCuotaResponse>();
            var due = request.FirstPaymentDate.Date;
            decimal sumAmount = 0, sumInterest = 0;

            for (var i = 1; i < count; i++)
            {
                cuotas.Add(new LetraVariaCuotaResponse { DueDate = due, Amount = baseAmount, Interest = baseInterest });
                sumAmount += baseAmount;
                sumInterest += baseInterest;
                due = due.AddDays(DaysBetweenInstallments);
            }

            cuotas.Add(new LetraVariaCuotaResponse
            {
                DueDate = due,
                Amount = DocumentoVentaRules.Round2(financed - sumAmount),
                Interest = DocumentoVentaRules.Round2(interestTotal - sumInterest)
            });

            if (cuotas.Any(c => c.Amount <= 0))
            {
                throw new InvalidOperationException("Con esa cantidad de letras alguna cuota resulta en cero o negativa. Reduzca la cantidad.");
            }

            var response = new GenerarLetrasVariasResponse
            {
                Saved = save,
                MonedaCode = moneda,
                ExchangeRate = rate,
                TotalDocuments = totalDocuments,
                InitialAmount = initial,
                TotalLetras = financed,
                Letras = cuotas
            };

            if (!save)
            {
                return response;
            }

            // ---- Guardar: cancelación (tipo 11) + líneas + letras
            var numbers = await LetraVentaRules.ReserveNumbersAsync(_uow, plantaCode, count, ct);
            var code = await CancelacionCodigoGenerator.NextAsync(_uow, ct);

            var cancelacion = new CancelacionVenta
            {
                Code = code,
                PlantaCode = plantaCode,
                ClienteCode = clienteCode,
                Date = date,
                CobradorCode = cobradorCode,
                MonedaCode = moneda,
                ExchangeRate = rate,
                TipoPagoCode = TipoPagoLetrasVarias,
                PaymentNumber = "VARIOS",
                IsDeferred = false,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                TotalAmount = totalDocuments,
                UsedAmount = 0m,
                DifferenceAmount = 0m,
                AvailableAmount = 0m,
                InstallmentCount = count,
                InstallmentType = request.InstallmentType,
                FirstPaymentDate = request.FirstPaymentDate.Date,
                InitialAmount = initial,
                Interest = interestTotal,
                Status = EstadoCancelacion.Pendiente
            };

            var item = 0;
            foreach (var l in lines)
            {
                item++;
                cancelacion.Details.Add(new CancelacionVentaDetalle
                {
                    CancelacionCode = code,
                    Item = item.ToString("000"),
                    PlantaCode = l.Doc.PlantaCode ?? plantaCode,
                    ClienteCode = clienteCode,
                    CancelacionDate = date,
                    TypeCode = l.Doc.TypeCode,
                    DocumentNumber = l.Doc.Number,
                    DocumentBancoCode = l.Doc.BancoCode,
                    DocumentDate = l.Doc.IssueDate,
                    DocumentCurrencyCode = l.Doc.CurrencyCode,
                    DocumentExchangeRate = l.Doc.ExchangeRate,
                    NetAmount = l.Sign * l.Doc.Total,
                    PaidAmount = l.Paid,
                    PaidSoles = l.PaidSoles,
                    DifferenceSoles = 0m,
                    ProvisionEntryNumber = l.Doc.ProvisionEntry,
                    ObraCode = l.Doc.ObraCode,
                    IsDoubtful = false,
                    DoubtfulExchangeRate = 0m,
                    IsDetraction = false
                });

                // Legacy Documento_Caj: el documento queda reservado y sale de la lista de pendientes.
                l.Doc.SetCashDocument?.Invoke(code);
            }

            await _uow.Facturacion.Transacciones.CancelacionesVenta.AddAsync(cancelacion, ct);

            for (var i = 0; i < count; i++)
            {
                cuotas[i].Number = numbers[i];

                await _uow.Facturacion.Transacciones.LetrasVenta.AddAsync(new LetraVenta
                {
                    PlantaCode = plantaCode,
                    Number = numbers[i],
                    ClienteCode = clienteCode,
                    BancoCode = null,
                    IssueDate = date,
                    DueDate = cuotas[i].DueDate,
                    CobradorCode = cobradorCode,
                    MonedaCode = moneda,
                    ExchangeRate = rate,
                    TotalAmount = cuotas[i].Amount,
                    PendingAmount = cuotas[i].Amount,
                    Interest = cuotas[i].Interest,
                    Description = null,
                    CancelacionCode = code,
                    IsPrinted = false,
                    AcceptedByClient = false,
                    Status = EstadoLetra.Facturado
                }, ct);
            }

            await _uow.SaveChangesAsync(ct);

            response.CancelacionCode = code;

            return response;
        }
    }
}
