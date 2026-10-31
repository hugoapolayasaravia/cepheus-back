using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.ExportSctrSaluds
{
    public class ExportSctrSaludsQueryHandler : IRequestHandler<ExportSctrSaludsQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<SctrSaludResponse>> Catalogo = new List<ExportColumn<SctrSaludResponse>>
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

        public ExportSctrSaludsQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportSctrSaludsQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Rrhh.Catalogos.SctrsSalud.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.Code.ToLower().Contains(search) ||
                    s.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(s => new SctrSaludResponse
            {
                Code = s.Code,
                Name = s.Name,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy,
                RowVersion = s.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "SCTR salud", "sctr_salud", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
