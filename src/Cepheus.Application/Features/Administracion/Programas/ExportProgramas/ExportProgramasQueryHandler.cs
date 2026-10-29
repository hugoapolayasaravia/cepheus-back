using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Programas.ExportProgramas
{
    public class ExportProgramasQueryHandler : IRequestHandler<ExportProgramasQuery, ExportFileResponse>
    {
        private sealed record Fila(
            string Submodulo, int DisplayOrder, string Code, string Name, string? Icon, string? Tooltip,
            string? Route, string Permisos, bool IsActive,
            DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("submodulo", "Submódulo", f => f.Submodulo),
            new("displayOrder", "Orden", f => f.DisplayOrder),
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("route", "Ruta", f => f.Route),
            new("icon", "Icono", f => f.Icon),
            new("tooltip", "Tooltip", f => f.Tooltip),
            new("permissions", "Permisos", f => f.Permisos),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy)
        };

        private static readonly string[] PorDefecto =
            { "submodulo", "displayOrder", "code", "name", "route", "permissions", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportProgramasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportProgramasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Administracion.Programas.Query().AsNoTracking();
            string? filtroSubmodulo = null;

            if (request.SubmoduloId.HasValue)
            {
                query = query.Where(p => p.SubmoduloId == request.SubmoduloId.Value);
                var nombre = await _uow.Administracion.Submodulos.Query().AsNoTracking()
                    .Where(s => s.Id == request.SubmoduloId.Value)
                    .Select(s => s.Modulo.Name + " › " + s.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                filtroSubmodulo = $"Submódulo: {nombre ?? request.SubmoduloId.Value.ToString()}";
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p => p.Code.ToLower().Contains(search) || p.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            // Los permisos se traen como lista y se unen en memoria (string.Join no se traduce a SQL).
            var crudas = await sorted
                .Select(p => new
                {
                    Submodulo = p.Submodulo.Modulo.Name + " › " + p.Submodulo.Name,
                    p.DisplayOrder, p.Code, p.Name, p.Icon, p.Tooltip, p.Route, p.IsActive,
                    p.CreatedAt, p.CreatedBy, p.UpdatedAt, p.UpdatedBy,
                    Permisos = p.Permissions.Select(x => x.Code).OrderBy(x => x).ToList()
                })
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = crudas
                .Select(p => new Fila(p.Submodulo, p.DisplayOrder, p.Code, p.Name, p.Icon, p.Tooltip, p.Route,
                    string.Join(", ", p.Permisos), p.IsActive, p.CreatedAt, p.CreatedBy, p.UpdatedAt, p.UpdatedBy))
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Programas", "programas", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive, filtroSubmodulo));
        }
    }
}
