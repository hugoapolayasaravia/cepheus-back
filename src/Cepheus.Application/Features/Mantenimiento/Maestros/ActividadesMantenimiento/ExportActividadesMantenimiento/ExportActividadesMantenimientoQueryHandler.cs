using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.ExportActividadesMantenimiento
{
    public class ExportActividadesMantenimientoQueryHandler : IRequestHandler<ExportActividadesMantenimientoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ActividadMantenimientoResponse>> Catalogo = new List<ExportColumn<ActividadMantenimientoResponse>>
        {
            new("actividadCode", "Actividad", f => f.ActividadCode),
            new("actividadName", "Nombre de la actividad", f => f.ActividadName),
            new("actividadCubiertaCode", "Actividad cubierta", f => f.ActividadCubiertaCode),
            new("actividadCubiertaName", "Nombre de la actividad cubierta", f => f.ActividadCubiertaName),
            new("prioridad", "Prioridad", f => f.Prioridad),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "actividadCode", "actividadName", "actividadCubiertaCode", "actividadCubiertaName", "prioridad" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportActividadesMantenimientoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportActividadesMantenimientoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.ActividadCode))
            {
                var act = request.ActividadCode.Trim().ToUpper();
                query = query.Where(a => a.ActividadCode == act);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToUpper();
                query = query.Where(a => a.ActividadCode.Contains(search) || a.ActividadCubiertaCode.Contains(search));
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Prioridad).ThenBy(a => a.ActividadCode).ThenBy(a => a.ActividadCubiertaCode)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(ActividadMantenimientoProjection.ToResponse)
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Actividades de mantenimiento", "actividades_mantenimiento", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null,
                    string.IsNullOrWhiteSpace(request.ActividadCode) ? null : $"Actividad: {request.ActividadCode.Trim().ToUpper()}"));
        }
    }
}
