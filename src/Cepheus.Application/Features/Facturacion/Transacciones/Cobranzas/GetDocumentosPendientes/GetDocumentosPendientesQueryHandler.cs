using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cobranzas.GetDocumentosPendientes
{
    public class GetDocumentosPendientesQueryHandler
        : IRequestHandler<GetDocumentosPendientesQuery, List<DocumentoPendienteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetDocumentosPendientesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DocumentoPendienteResponse>> Handle(
            GetDocumentosPendientesQuery request, CancellationToken cancellationToken)
        {
            var cliente = string.IsNullOrWhiteSpace(request.ClienteCode) || request.ClienteCode.Trim() == "%"
                ? null
                : request.ClienteCode.Trim();
            var planta = string.IsNullOrWhiteSpace(request.PlantaCode) ? null : request.PlantaCode.Trim().ToUpperInvariant();
            var type = string.IsNullOrWhiteSpace(request.TypeCode) ? "00" : request.TypeCode.Trim();
            DateTime? from = request.DateFrom?.Date;
            DateTime? to = request.DateTo?.Date.AddDays(1);

            if (type != "00" && !CobranzaTipoDocumento.Todos.Contains(type))
            {
                throw new InvalidOperationException(
                    $"El tipo de documento debe ser 00 (todos) o uno de: {string.Join(", ", CobranzaTipoDocumento.Todos)}.");
            }

            var t = _uow.Facturacion.Transacciones;
            var result = new List<DocumentoPendienteResponse>();

            // Facturas y boletas
            foreach (var code in new[] { CobranzaTipoDocumento.Factura, CobranzaTipoDocumento.Boleta })
            {
                if (type != "00" && type != code) continue;

                var q = t.ComprobantesVenta.Query().AsNoTracking()
                    .Where(c => c.TypeCode == code && c.Status == EstadoComprobanteVenta.Facturado && c.IsPrinted
                                && c.ProvisionEntry != null && c.ProvisionEntry != "" && c.CashDocument == null);
                if (cliente != null) q = q.Where(c => c.ClienteCode == cliente);
                if (planta != null) q = q.Where(c => c.PlantCode == planta);
                if (from.HasValue) q = q.Where(c => c.IssueDate >= from.Value && c.IssueDate < to!.Value);

                result.AddRange((await q.Select(c => new
                {
                    c.TypeCode, c.PlantCode, c.Number, c.ClienteCode, c.IssueDate, c.PaymentForm.Days,
                    c.CurrencyCode, c.ExchangeRate, c.TotalAmount, c.PendingAmount, c.ObraCode, c.ProvisionEntry
                }).ToListAsync(cancellationToken)).Select(c => new DocumentoPendienteResponse
                {
                    TypeCode = c.TypeCode, PlantaCode = c.PlantCode, Number = c.Number, ClienteCode = c.ClienteCode,
                    IssueDate = c.IssueDate, DueDate = c.IssueDate.AddDays(c.Days), CurrencyCode = c.CurrencyCode,
                    ExchangeRate = c.ExchangeRate, Total = c.TotalAmount, Pending = c.PendingAmount,
                    ObraCode = c.ObraCode, ProvisionEntryNumber = c.ProvisionEntry
                }));
            }

            // Notas de crédito (en negativo)
            if (type is "00" or CobranzaTipoDocumento.NotaCredito)
            {
                var q = t.NotasCreditoVenta.Query().AsNoTracking()
                    .Where(n => n.Status == EstadoNotaCredito.Facturado && n.IsPrinted
                                && n.ProvisionEntryNumber != null && n.ProvisionEntryNumber != "" && n.CashDocument == null);
                if (cliente != null) q = q.Where(n => n.ClienteCode == cliente);
                if (planta != null) q = q.Where(n => n.PlantaCode == planta);
                if (from.HasValue) q = q.Where(n => n.IssueDate >= from.Value && n.IssueDate < to!.Value);

                result.AddRange((await q.Select(n => new
                {
                    n.PlantaCode, n.Number, n.ClienteCode, n.IssueDate, n.FormaPagoVenta.Days,
                    n.MonedaCode, n.ExchangeRate, n.TotalAmount, n.PendingAmount, n.ObraCode, n.ProvisionEntryNumber
                }).ToListAsync(cancellationToken)).Select(n => new DocumentoPendienteResponse
                {
                    TypeCode = CobranzaTipoDocumento.NotaCredito, PlantaCode = n.PlantaCode, Number = n.Number,
                    ClienteCode = n.ClienteCode, IssueDate = n.IssueDate, DueDate = n.IssueDate.AddDays(n.Days),
                    CurrencyCode = n.MonedaCode, ExchangeRate = n.ExchangeRate, Total = -n.TotalAmount,
                    Pending = -n.PendingAmount, ObraCode = n.ObraCode, ProvisionEntryNumber = n.ProvisionEntryNumber
                }));
            }

            // Notas de débito
            if (type is "00" or CobranzaTipoDocumento.NotaDebito)
            {
                var q = t.NotasDebitoVenta.Query().AsNoTracking()
                    .Where(n => n.Status == EstadoNotaDebito.Facturado && n.IsPrinted
                                && n.ProvisionEntryNumber != null && n.ProvisionEntryNumber != "");
                if (cliente != null) q = q.Where(n => n.ClienteCode == cliente);
                if (planta != null) q = q.Where(n => n.PlantaCode == planta);
                if (from.HasValue) q = q.Where(n => n.IssueDate >= from.Value && n.IssueDate < to!.Value);

                result.AddRange((await q.Select(n => new
                {
                    n.PlantaCode, n.Number, n.ClienteCode, n.IssueDate, n.FormaPagoVenta.Days,
                    n.MonedaCode, n.ExchangeRate, n.TotalAmount, n.PendingAmount, n.ObraCode, n.ProvisionEntryNumber
                }).ToListAsync(cancellationToken)).Select(n => new DocumentoPendienteResponse
                {
                    TypeCode = CobranzaTipoDocumento.NotaDebito, PlantaCode = n.PlantaCode, Number = n.Number,
                    ClienteCode = n.ClienteCode, IssueDate = n.IssueDate, DueDate = n.IssueDate.AddDays(n.Days),
                    CurrencyCode = n.MonedaCode, ExchangeRate = n.ExchangeRate, Total = n.TotalAmount,
                    Pending = n.PendingAmount, ObraCode = n.ObraCode, ProvisionEntryNumber = n.ProvisionEntryNumber
                }));
            }

            // Letras (obra "001" como el legacy)
            if (type is "00" or CobranzaTipoDocumento.Letra)
            {
                var q = t.LetrasVenta.Query().AsNoTracking()
                    .Where(l => l.Status == EstadoLetra.Facturado && l.IsPrinted
                                && l.ProvisionEntryNumber != null && l.ProvisionEntryNumber != "");
                if (cliente != null) q = q.Where(l => l.ClienteCode == cliente);
                if (planta != null) q = q.Where(l => l.PlantaCode == planta);
                if (from.HasValue) q = q.Where(l => l.IssueDate >= from.Value && l.IssueDate < to!.Value);

                result.AddRange((await q.ToListAsync(cancellationToken)).Select(l => new DocumentoPendienteResponse
                {
                    TypeCode = CobranzaTipoDocumento.Letra, PlantaCode = l.PlantaCode, Number = l.Number,
                    ClienteCode = l.ClienteCode, IssueDate = l.IssueDate, DueDate = l.DueDate,
                    CurrencyCode = l.MonedaCode, ExchangeRate = l.ExchangeRate, Total = l.TotalAmount,
                    Pending = l.PendingAmount, ObraCode = "001", ProvisionEntryNumber = l.ProvisionEntryNumber
                }));
            }

            // Cheques rebotados
            if (type is "00" or CobranzaTipoDocumento.Cheque)
            {
                var q = t.ChequesVenta.Query().AsNoTracking()
                    .Where(c => c.Status == EstadoCheque.Rebotado
                                && c.ProvisionEntryNumber != null && c.ProvisionEntryNumber != "");
                if (cliente != null) q = q.Where(c => c.ClienteCode == cliente);
                if (planta != null) q = q.Where(c => c.PlantaCode == planta);
                if (from.HasValue) q = q.Where(c => c.IssueDate >= from.Value && c.IssueDate < to!.Value);

                result.AddRange((await q.ToListAsync(cancellationToken)).Select(c => new DocumentoPendienteResponse
                {
                    TypeCode = CobranzaTipoDocumento.Cheque, PlantaCode = c.PlantaCode, Number = c.Number,
                    BancoCode = c.BancoCode, ClienteCode = c.ClienteCode, IssueDate = c.IssueDate, DueDate = c.PaymentDate,
                    CurrencyCode = c.MonedaCode, ExchangeRate = c.ExchangeRate, Total = c.TotalAmount,
                    Pending = c.PendingAmount, ObraCode = "001", ProvisionEntryNumber = c.ProvisionEntryNumber
                }));
            }

            return result
                .OrderBy(d => type == "00" ? d.Number : d.PlantaCode + d.Number)
                .ThenBy(d => d.TypeCode)
                .ToList();
        }
    }
}
