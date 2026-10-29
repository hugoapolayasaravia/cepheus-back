using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Submodulos.ExportSubmodulos
{
    public class ExportSubmodulosQueryHandler : IRequestHandler<ExportSubmodulosQuery, ExportFileResponse>
    {
        private sealed record Fila(
            string Modulo, int DisplayOrder, string Code, string Name, string? Icon, string? Tooltip, bool IsActive,
            DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("modulo", "Módulo", f => f.Modulo),
            new("displayOrder", "Orden", f => f.DisplayOrder),
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("icon", "Icono", f => f.Icon),
            new("tooltip", "Tooltip", f => f.Tooltip),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy)
        };

        private static readonly string[] PorDefecto = { "modulo", "displayOrder", "code", "name", "icon", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportSubmodulosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportSubmodulosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Administracion.Submodulos.Query().AsNoTracking();
            string? filtroModulo = null;

            if (request.ModuloId.HasValue)
            {
                query = query.Where(s => s.ModuloId == request.ModuloId.Value);
                var nombre = await _uow.Administracion.Modulos.Query().AsNoTracking()
                    .Where(m => m.Id == request.ModuloId.Value)
                    .Select(m => m.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                filtroModulo = $"Módulo: {nombre ?? request.ModuloId.Value.ToString()}";
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s => s.Code.ToLower().Contains(search) || s.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Name)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(s => new Fila(s.Modulo.Name, s.DisplayOrder, s.Code, s.Name, s.Icon, s.Tooltip, s.IsActive,
                    s.CreatedAt, s.CreatedBy, s.UpdatedAt, s.UpdatedBy))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Submódulos", "submodulos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive, filtroModulo));
        }
    }
}
