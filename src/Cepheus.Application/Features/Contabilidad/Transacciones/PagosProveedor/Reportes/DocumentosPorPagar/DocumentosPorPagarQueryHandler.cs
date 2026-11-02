using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.DocumentosPorPagar
{
    public class DocumentosPorPagarQueryHandler
        : IRequestHandler<DocumentosPorPagarQuery, List<DocumentoPorPagarRowResponse>>
    {
        private const string TodasLasPlantas = "ZZ";

        private readonly IUnitOfWork _uow;

        public DocumentosPorPagarQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DocumentoPorPagarRowResponse>> Handle(
            DocumentosPorPagarQuery request, CancellationToken cancellationToken)
        {
            // Legacy: sin fecha o con año menor a 1990, el corte es hoy.
            var corte = request.FechaCorte.HasValue && request.FechaCorte.Value.Year >= 1990
                ? request.FechaCorte.Value.Date
                : DateTime.Today;

            var planta = string.IsNullOrWhiteSpace(request.PlantaCode) || request.PlantaCode.Trim().ToUpper() == TodasLasPlantas
                ? null
                : request.PlantaCode.Trim().ToUpper();
            var proveedor = string.IsNullOrWhiteSpace(request.ProveedorCode) ? null : request.ProveedorCode.Trim();

            // Documentos recibidos hasta el corte, en soles. La planta del documento es la de su primera línea
            // (legacy: com_planta_doc).
            var docsQuery = _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .AsNoTracking()
                .Where(d => d.FechaRecepcion <= corte && d.MonedaCode == PagoProveedorRules.MonedaBase);

            if (proveedor is not null)
            {
                docsQuery = docsQuery.Where(d => d.ProveedorCode == proveedor);
            }

            var docs = await docsQuery
                .Select(d => new
                {
                    Doc = d,
                    ProveedorRuc = d.Proveedor.DocumentNumber,
                    ProveedorName = d.Proveedor.LegalName,
                    Planta = d.Detalles.OrderBy(x => x.ItemNumber).Select(x => x.PlantaCode).FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            if (planta is not null)
            {
                docs = docs.Where(x => x.Planta == planta).ToList();
            }

            // Pagos hechos DESPUÉS del corte: se descuentan de lo pagado para ver el saldo a esa fecha.
            var pagosPosteriores = _uow.Contabilidad.Transacciones.PagoProveedorDetalles.Query()
                .AsNoTracking()
                .Where(l => l.Pago.FechaEmision > corte);

            if (proveedor is not null)
            {
                pagosPosteriores = pagosPosteriores.Where(l => l.ProveedorCode == proveedor);
            }

            var posteriores = (await pagosPosteriores
                    .Select(l => new { l.ComprobantePagoCode, l.NumeroDocumento, l.ProveedorCode, l.PagoSol })
                    .ToListAsync(cancellationToken))
                .GroupBy(l => PagoProveedorRecalculator.Key(l.ComprobantePagoCode, l.NumeroDocumento, l.ProveedorCode))
                .ToDictionary(g => g.Key, g => g.Sum(l => Math.Abs(l.PagoSol)));

            var rows = new List<DocumentoPorPagarRowResponse>();
            foreach (var x in docs)
            {
                var d = x.Doc;
                var key = PagoProveedorRecalculator.Key(d.ComprobantePagoCode, d.NumeroDocumento, d.ProveedorCode);
                var pagado = d.PagadoSol - posteriores.GetValueOrDefault(key);
                var pendiente = d.TotalSol - pagado;

                if (pendiente <= 0)
                {
                    continue;
                }

                var dias = (corte - d.FechaRecepcion.Date).Days;

                rows.Add(new DocumentoPorPagarRowResponse
                {
                    PlantaCode = x.Planta ?? string.Empty,
                    ProveedorCode = d.ProveedorCode,
                    ProveedorRuc = x.ProveedorRuc ?? string.Empty,
                    ProveedorName = x.ProveedorName ?? string.Empty,
                    FormaPagoCode = d.FormaPagoCode,
                    FechaEmision = d.FechaEmision,
                    FechaVencimiento = d.FechaVencimiento,
                    FechaRecepcion = d.FechaRecepcion,
                    FechaCorte = corte,
                    ComprobantePagoCode = d.ComprobantePagoCode,
                    NumeroDocumento = d.NumeroDocumento,
                    MonedaCode = d.MonedaCode,
                    TotalSol = d.TotalSol,
                    PagadoSol = pagado,
                    PendienteSol = pendiente,
                    Dias = dias,
                    Periodo1Sol = dias is >= 0 and <= 7 ? pendiente : 0,
                    Periodo2Sol = dias is >= 8 and <= 15 ? pendiente : 0,
                    Periodo3Sol = dias is >= 16 and <= 30 ? pendiente : 0,
                    Periodo4Sol = dias is >= 31 and <= 45 ? pendiente : 0,
                    Periodo5Sol = dias > 45 ? pendiente : 0
                });
            }

            return rows
                .OrderBy(r => r.ProveedorName)
                .ThenBy(r => r.ComprobantePagoCode)
                .ThenBy(r => r.NumeroDocumento)
                .ToList();
        }
    }
}
