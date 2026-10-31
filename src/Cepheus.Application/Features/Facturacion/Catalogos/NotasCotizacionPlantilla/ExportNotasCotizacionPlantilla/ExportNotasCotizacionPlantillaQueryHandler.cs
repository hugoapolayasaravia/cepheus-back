using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.ExportNotasCotizacionPlantilla
{
    public class ExportNotasCotizacionPlantillaQueryHandler : IRequestHandler<ExportNotasCotizacionPlantillaQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<NotaCotizacionPlantillaResponse>> Catalogo = new List<ExportColumn<NotaCotizacionPlantillaResponse>>
        {
            new("negocioCode", "Negocio", f => f.NegocioCode),
            new("code", "Código", f => f.Code),
            new("description", "Descripción", f => f.Description),
            new("option", "Opción", f => f.Option.ToString()),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "negocioCode", "code", "description", "option", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportNotasCotizacionPlantillaQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportNotasCotizacionPlantillaQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.NegocioCode))
            {
                query = query.Where(t => t.NegocioCode == request.NegocioCode);
            }

            if (request.Option.HasValue)
            {
                query = query.Where(t => t.Option == request.Option.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t => t.Description.ToLower().Contains(search));
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.NegocioCode).ThenBy(t => t.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(t => new NotaCotizacionPlantillaResponse
            {
                NegocioCode = t.NegocioCode,
                Code = t.Code,
                Description = t.Description,
                Option = t.Option,
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
                "Notas de cotización (plantillas)", "notas_cotizacion_plantilla", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.NegocioCode) ? null : $"Negocio: {request.NegocioCode.Trim()}",
                    request.Option.HasValue ? $"Opción: {request.Option.Value}" : null));
        }
    }
}
