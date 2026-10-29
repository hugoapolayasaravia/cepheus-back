using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.ExportTiposCambio
{
    public class ExportTiposCambioQueryHandler : IRequestHandler<ExportTiposCambioQuery, ExportFileResponse>
    {
        private sealed record Fila(string MonedaCode, string? MonedaName, DateOnly Date, decimal BuyRate, decimal SellRate, DateTime CreatedAt, DateTime? UpdatedAt);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("monedaCode", "Moneda", f => f.MonedaCode),
            new("monedaName", "Nombre de moneda", f => f.MonedaName),
            new("date", "Fecha", f => f.Date.ToString("dd/MM/yyyy")),
            new("buyRate", "Compra", f => f.BuyRate),
            new("sellRate", "Venta", f => f.SellRate),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "monedaCode", "date", "buyRate", "sellRate" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportTiposCambioQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportTiposCambioQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Comunes.TiposCambio.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.MonedaCode))
            {
                var moneda = TipoCambioRules.Normalizar(request.MonedaCode);
                query = query.Where(x => x.MonedaCode == moneda);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.Date >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.Date <= request.ToDate.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.Date).ThenBy(x => x.MonedaCode)
                : query.ApplySort(request.SortBy, request.SortDesc ?? true);

            var filas = await sorted
                .Select(x => new Fila(x.MonedaCode, x.Moneda.Name, x.Date, x.BuyRate, x.SellRate, x.CreatedAt, x.UpdatedAt))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Tipo de cambio", "tipo_cambio", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, null,
                !string.IsNullOrWhiteSpace(request.MonedaCode) ? $"Moneda: {request.MonedaCode}" : null,
                request.FromDate.HasValue ? $"Desde: {request.FromDate!.Value:dd/MM/yyyy}" : null,
                request.ToDate.HasValue ? $"Hasta: {request.ToDate!.Value:dd/MM/yyyy}" : null));
        }
    }
}
