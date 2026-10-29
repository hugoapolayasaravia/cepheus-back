using System.Globalization;
using System.Text;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GenerarArchivoDetraccion
{
    public class GenerarArchivoDetraccionCommandHandler
        : IRequestHandler<GenerarArchivoDetraccionCommand, ExportFileResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IDetraccionSettings _settings;

        public GenerarArchivoDetraccionCommandHandler(IUnitOfWork uow, IDetraccionSettings settings)
        {
            _uow = uow;
            _settings = settings;
        }

        public async Task<ExportFileResponse> Handle(
            GenerarArchivoDetraccionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);

            var detraccion = await _uow.Contabilidad.Transacciones.Detracciones.Query()
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot, cancellationToken);

            if (detraccion is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            var items = detraccion.Items
                .Where(i => i.DepositSol > 0m)
                .OrderBy(i => i.ProveedorCode, StringComparer.Ordinal)
                .ThenBy(i => i.ItemNumber)
                .ToList();

            if (items.Count == 0)
            {
                throw new InvalidOperationException(
                    "El lote no tiene ítems con depósito mayor a cero: no hay nada que exportar.");
            }

            var documents = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot && d.IncludeInExport)
                .Select(d => new
                {
                    d.ProveedorCode,
                    d.ItemNumber,
                    d.ComprobantePagoCode,
                    d.NumeroDocumento,
                    d.DocumentoCompra.FechaEmision
                })
                .ToListAsync(cancellationToken);

            // Periodo del ítem = mes de emisión del primer documento exportado del ítem (por comprobante y
            // número). El legacy tomaba un documento cualquiera (TOP 1 sin orden).
            var periods = documents
                .GroupBy(d => (d.ProveedorCode, d.ItemNumber))
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.ComprobantePagoCode, StringComparer.Ordinal)
                          .ThenBy(x => x.NumeroDocumento, StringComparer.Ordinal)
                          .First()
                          .FechaEmision.ToString("yyyyMM", CultureInfo.InvariantCulture));

            var archivoItems = items
                .Select(i => new DetraccionArchivoItem(
                    i.ProveedorRuc,
                    i.TipoBienCode,
                    i.AccountNumber,
                    i.TipoOperacionCode,
                    i.DepositSol,
                    periods.TryGetValue((i.ProveedorCode, i.ItemNumber), out var p)
                        ? p
                        : throw new InvalidOperationException(
                            $"El ítem {i.ItemNumber} ({i.ProveedorCode}) no tiene documentos marcados para exportar.")))
                .ToList();

            var lines = DetraccionArchivoBuilder.BuildLines(year, lot, archivoItems, _settings);

            if (detraccion.Status == EstadoDetraccion.Pendiente)
            {
                detraccion.DepositDate = DetraccionRules.TodayLocal();
                detraccion.Status = EstadoDetraccion.Emitido;

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new InvalidOperationException(
                        "La detracción fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
                }

                _uow.ClearTracking();
            }

            // Archivo ANSI con fin de línea CRLF (como lo escribía PowerBuilder con SaveAs Text).
            var content = Encoding.Latin1.GetBytes(string.Join("\r\n", lines) + "\r\n");
            var ruc = _settings.AdquirienteRuc.Trim();

            return new ExportFileResponse(content, "text/plain", $"D{ruc}{year[^2..]}{lot}.txt");
        }
    }
}
