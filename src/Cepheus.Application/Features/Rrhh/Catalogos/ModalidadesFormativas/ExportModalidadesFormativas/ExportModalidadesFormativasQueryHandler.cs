using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.ExportModalidadesFormativas
{
    public class ExportModalidadesFormativasQueryHandler : IRequestHandler<ExportModalidadesFormativasQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ModalidadFormativaResponse>> Catalogo = new List<ExportColumn<ModalidadFormativaResponse>>
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

        public ExportModalidadesFormativasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportModalidadesFormativasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Rrhh.Catalogos.ModalidadesFormativas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(m =>
                    m.Code.ToLower().Contains(search) ||
                    m.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(m => m.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(m => m.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(m => new ModalidadFormativaResponse
            {
                Code = m.Code,
                Name = m.Name,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedBy = m.UpdatedBy,
                RowVersion = m.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Modalidades formativas", "modalidades_formativas", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
