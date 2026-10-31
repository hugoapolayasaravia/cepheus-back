using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ExportPlanesPreventivo
{
    public class ExportPlanesPreventivoQueryHandler : IRequestHandler<ExportPlanesPreventivoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<PlanPreventivoResponse>> Catalogo = new List<ExportColumn<PlanPreventivoResponse>>
        {
            new("plantaCode", "Planta", f => f.PlantaCode),
            new("code", "Código", f => f.Code),
            new("description", "Descripción", f => f.Description),
            new("equipoCode", "Equipo", f => f.EquipoCode),
            new("equipoName", "Nombre del equipo", f => f.EquipoName),
            new("especialidadName", "Especialidad", f => f.EspecialidadName),
            new("responsableName", "Responsable", f => f.ResponsableName),
            new("subCentroCostoCode", "Subcentro de costo", f => f.SubCentroCostoCode),
            new("subCentroEjecutorName", "Subcentro ejecutor", f => f.SubCentroEjecutorName),
            new("actividadName", "Actividad", f => f.ActividadName),
            new("frecuencia", "Frecuencia", f => f.Frecuencia),
            new("unidadFrecuencia", "Unidad", f => f.UnidadFrecuencia.ToString()),
            new("factor", "Factor", f => f.Factor),
            new("fechaInicio", "Inicio", f => f.FechaInicio?.ToString("dd/MM/yyyy")),
            new("fechaTermino", "Término", f => f.FechaTermino?.ToString("dd/MM/yyyy")),
            new("frecuenciaGama", "Frecuencia gama", f => f.FrecuenciaGama),
            new("tiempoRestante", "Tiempo restante", f => f.TiempoRestante),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "plantaCode", "code", "description", "equipoCode", "equipoName", "actividadName", "frecuencia", "unidadFrecuencia", "fechaInicio", "fechaTermino" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPlanesPreventivoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPlanesPreventivoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = request.PlantaCode.Trim().ToUpper();
                query = query.Where(x => x.PlantaCode == planta);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim().ToUpper();
                query = query.Where(x => x.Code == code);
            }

            if (!string.IsNullOrWhiteSpace(request.EquipoCode))
            {
                var equipo = request.EquipoCode.Trim().ToUpper();
                query = query.Where(x => x.EquipoCode.Contains(equipo));
            }

            if (!string.IsNullOrWhiteSpace(request.ResponsableCode))
            {
                var responsable = request.ResponsableCode.Trim().ToUpper();
                query = query.Where(x => x.ResponsableCode == responsable);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Description.ToLower().Contains(search) ||
                    x.Equipo.Name.ToLower().Contains(search));
            }

            // Mismo orden que el listado (legacy: ORDER BY Codigo_Pla DESC, Codigo_Mpv DESC)
            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.PlantaCode).ThenByDescending(x => x.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(PlanPreventivoProjection.ToResponse)
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Planes de mantenimiento preventivo", "planes_preventivo", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, null,
                    string.IsNullOrWhiteSpace(request.PlantaCode) ? null : $"Planta: {request.PlantaCode.Trim().ToUpper()}",
                    string.IsNullOrWhiteSpace(request.Code) ? null : $"Código: {request.Code.Trim().ToUpper()}",
                    string.IsNullOrWhiteSpace(request.EquipoCode) ? null : $"Equipo: {request.EquipoCode.Trim().ToUpper()}",
                    string.IsNullOrWhiteSpace(request.ResponsableCode) ? null : $"Responsable: {request.ResponsableCode.Trim().ToUpper()}"));
        }
    }
}
