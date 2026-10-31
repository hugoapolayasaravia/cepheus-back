using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.ExportEquipos
{
    public class ExportEquiposQueryHandler : IRequestHandler<ExportEquiposQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string Name, int Nivel, string? SubCentroCostoCode, bool IsActive, DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Descripción", f => f.Name),
            new("nivel", "Nivel", f => f.Nivel),
            new("subCentroCostoCode", "Subcentro de costo", f => f.SubCentroCostoCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "nivel", "subCentroCostoCode", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportEquiposQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportEquiposQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Mantenimiento.Maestros.Equipos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(e =>
                    e.Code.ToLower().Contains(search) ||
                    e.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
            {
                var subCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpper();
                query = query.Where(e => e.SubCentroCostoCode == subCentroCostoCode);
            }

            if (request.Nivel.HasValue)
            {
                query = query.Where(e => e.Nivel == request.Nivel.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(e => e.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(e => new Fila(e.Code, e.Name, e.Nivel, e.SubCentroCostoCode, e.IsActive, e.CreatedAt, e.CreatedBy, e.UpdatedAt, e.UpdatedBy))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Equipos", "equipos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.SubCentroCostoCode) ? null : $"Subcentro de costo: {request.SubCentroCostoCode.Trim().ToUpper()}",
                    request.Nivel.HasValue ? $"Nivel: {request.Nivel.Value}" : null));
        }
    }
}
