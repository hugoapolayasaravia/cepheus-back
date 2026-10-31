using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.ExportEstadosCiviles
{
    public class ExportEstadosCivilesQueryHandler : IRequestHandler<ExportEstadosCivilesQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<EstadoCivilResponse>> Catalogo = new List<ExportColumn<EstadoCivilResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportEstadosCivilesQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportEstadosCivilesQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Rrhh.Catalogos.EstadosCiviles.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(e =>
                    e.Code.ToLower().Contains(search) ||
                    e.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(e => e.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(e => new EstadoCivilResponse
            {
                Code = e.Code,
                Name = e.Name,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedBy = e.CreatedBy,
                UpdatedBy = e.UpdatedBy,
                RowVersion = e.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Estados civiles", "estados_civiles", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
