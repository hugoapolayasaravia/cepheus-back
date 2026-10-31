using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.ExportAnalisisVentas
{
    public class ExportAnalisisVentasQueryHandler : IRequestHandler<ExportAnalisisVentasQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<AnalisisVentaResponse>> Catalogo = new List<ExportColumn<AnalisisVentaResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("shortName", "Nombre corto", f => f.ShortName),
            new("segmentoVentasCode", "Segmento de ventas", f => f.SegmentoVentasCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "shortName", "segmentoVentasCode", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportAnalisisVentasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportAnalisisVentasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.AnalisisVentas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.Code.ToLower().Contains(search) ||
                    a.Name.ToLower().Contains(search) ||
                    (a.ShortName != null && a.ShortName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.SegmentoVentasCode))
            {
                var segmentoCode = request.SegmentoVentasCode.Trim().ToUpper();
                query = query.Where(a => a.SegmentoVentasCode == segmentoCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(a => new AnalisisVentaResponse
            {
                Code = a.Code,
                Name = a.Name,
                ShortName = a.ShortName,
                SegmentoVentasCode = a.SegmentoVentasCode,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                RowVersion = a.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Análisis de ventas", "analisis_ventas", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.SegmentoVentasCode) ? null : $"Segmento de ventas: {request.SegmentoVentasCode.Trim()}"));
        }
    }
}
