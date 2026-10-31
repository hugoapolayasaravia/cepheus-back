using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.ExportPrioridades
{
    public class ExportPrioridadesQueryHandler : IRequestHandler<ExportPrioridadesQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string Name, bool IsActive, DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPrioridadesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPrioridadesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Mantenimiento.Catalogos.Prioridades.Query().AsNoTracking();

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
                ? query.OrderBy(p => p.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(p => new Fila(p.Code, p.Name, p.IsActive, p.CreatedAt, p.CreatedBy, p.UpdatedAt, p.UpdatedBy))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Prioridades", "prioridades", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
