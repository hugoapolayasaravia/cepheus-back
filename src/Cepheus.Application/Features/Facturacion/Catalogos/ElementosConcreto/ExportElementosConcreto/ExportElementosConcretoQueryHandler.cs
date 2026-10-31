using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.ExportElementosConcreto
{
    public class ExportElementosConcretoQueryHandler : IRequestHandler<ExportElementosConcretoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ElementoConcretoResponse>> Catalogo = new List<ExportColumn<ElementoConcretoResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("usesBalance", "Usa balanza", f => ExportBuilder.SiNo(f.UsesBalance)),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "usesBalance", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportElementosConcretoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportElementosConcretoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.ElementosConcreto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    t.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(t => new ElementoConcretoResponse
            {
                Code = t.Code,
                Name = t.Name,
                UsesBalance = t.UsesBalance,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Elementos de concreto", "elementos_concreto", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
