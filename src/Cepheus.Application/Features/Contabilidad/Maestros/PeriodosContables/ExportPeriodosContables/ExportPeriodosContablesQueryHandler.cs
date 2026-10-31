using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ExportPeriodosContables
{
    public class ExportPeriodosContablesQueryHandler : IRequestHandler<ExportPeriodosContablesQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<PeriodoContableResponse>> Catalogo = new List<ExportColumn<PeriodoContableResponse>>
        {
            new("code", "Código", f => f.Code),
            new("year", "Año", f => f.Year),
            new("month", "Mes", f => f.Month),
            new("status", "Estado", f => f.Status.ToString()),
            new("closedAt", "Cerrado el", f => HoraLocal.Convertir(f.ClosedAt)),
            new("closedBy", "Cerrado por", f => f.ClosedBy),
            new("reopenedAt", "Reabierto el", f => HoraLocal.Convertir(f.ReopenedAt)),
            new("reopenedBy", "Reabierto por", f => f.ReopenedBy),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "year", "month", "status", "closedAt", "closedBy", "reopenedAt", "reopenedBy" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPeriodosContablesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPeriodosContablesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Contabilidad.Maestros.PeriodosContables.Query().AsNoTracking();

            if (request.Year.HasValue)
            {
                query = query.Where(x => x.Year == request.Year.Value);
            }

            if (request.Month.HasValue)
            {
                query = query.Where(x => x.Month == request.Month.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(PeriodoContableMapper.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(PeriodoContableMapper.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Períodos contables", "periodos_contables", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, null,
                    request.Year.HasValue ? $"Año: {request.Year.Value}" : null,
                    request.Month.HasValue ? $"Mes: {request.Month.Value}" : null,
                    request.Status.HasValue ? $"Estado: {request.Status.Value}" : null));
        }
    }
}
