using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.ExportActividades
{
    public class ExportActividadesQueryHandler : IRequestHandler<ExportActividadesQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string VerboCode, string VerboName, string ObjetoCode, string ObjetoName, bool IsActive, DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("verboActividadName", "Verbo", f => f.VerboName),
            new("objetoActividadName", "Objeto", f => f.ObjetoName),
            new("verboActividadCode", "Cód. verbo", f => f.VerboCode),
            new("objetoActividadCode", "Cód. objeto", f => f.ObjetoCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "verboActividadName", "objetoActividadName", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportActividadesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportActividadesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Mantenimiento.Maestros.Actividades.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.Code.ToLower().Contains(search) ||
                    a.VerboActividad.Name.ToLower().Contains(search) ||
                    a.ObjetoActividad.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.VerboActividadCode))
            {
                var verboCode = request.VerboActividadCode.Trim().ToUpper();
                query = query.Where(a => a.VerboActividadCode == verboCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ObjetoActividadCode))
            {
                var objetoCode = request.ObjetoActividadCode.Trim().ToUpper();
                query = query.Where(a => a.ObjetoActividadCode == objetoCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(a => new Fila(a.Code, a.VerboActividadCode, a.VerboActividad.Name, a.ObjetoActividadCode, a.ObjetoActividad.Name,
                    a.IsActive, a.CreatedAt, a.CreatedBy, a.UpdatedAt, a.UpdatedBy))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Actividades", "actividades", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.VerboActividadCode) ? null : $"Verbo: {request.VerboActividadCode.Trim().ToUpper()}",
                    string.IsNullOrWhiteSpace(request.ObjetoActividadCode) ? null : $"Objeto: {request.ObjetoActividadCode.Trim().ToUpper()}"));
        }
    }
}
