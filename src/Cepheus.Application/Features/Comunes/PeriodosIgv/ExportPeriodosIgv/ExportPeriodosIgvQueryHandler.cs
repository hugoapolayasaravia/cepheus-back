using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.ExportPeriodosIgv
{
    public class ExportPeriodosIgvQueryHandler : IRequestHandler<ExportPeriodosIgvQuery, ExportFileResponse>
    {
        private sealed record Fila(DateOnly FechaInicio, DateOnly? FechaFin, decimal PorcentajeIgv, DateTime CreatedAt, DateTime? UpdatedAt);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("fechaInicio", "Desde", f => f.FechaInicio.ToString("dd/MM/yyyy")),
            new("fechaFin", "Hasta", f => f.FechaFin.HasValue ? f.FechaFin.Value.ToString("dd/MM/yyyy") : "Vigente"),
            new("porcentajeIgv", "IGV (%)", f => f.PorcentajeIgv),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "fechaInicio", "fechaFin", "porcentajeIgv" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPeriodosIgvQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPeriodosIgvQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Comunes.PeriodosIgv.Query().AsNoTracking();

            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.FechaFin == null || x.FechaFin >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.FechaInicio <= request.ToDate.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.FechaInicio)
                : query.ApplySort(request.SortBy, request.SortDesc ?? true);

            var filas = await sorted
                .Select(x => new Fila(x.FechaInicio, x.FechaFin, x.PorcentajeIgv, x.CreatedAt, x.UpdatedAt))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Períodos de IGV", "periodos_igv", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, null,
                request.FromDate.HasValue ? $"Desde: {request.FromDate!.Value:dd/MM/yyyy}" : null,
                request.ToDate.HasValue ? $"Hasta: {request.ToDate!.Value:dd/MM/yyyy}" : null));
        }
    }
}
