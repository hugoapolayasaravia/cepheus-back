using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.ExportAlturasLosa
{
    public class ExportAlturasLosaQueryHandler : IRequestHandler<ExportAlturasLosaQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<AlturaLosaResponse>> Catalogo = new List<ExportColumn<AlturaLosaResponse>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("value", "Valor", f => f.Value),
            new("width", "Ancho", f => f.Width),
            new("productoTipoCode", "Tipo de producto", f => f.ProductoTipoCode),
            new("productoCode", "Producto", f => f.ProductoCode),
            new("polystyreneProductoTipoCode", "Tipo de producto (poliestireno)", f => f.PolystyreneProductoTipoCode),
            new("polystyreneProductoCode", "Producto (poliestireno)", f => f.PolystyreneProductoCode),
            new("polystyreneValue", "Valor (poliestireno)", f => f.PolystyreneValue),
            new("polystyreneWidth", "Ancho (poliestireno)", f => f.PolystyreneWidth),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "value", "width", "productoTipoCode", "productoCode", "polystyreneProductoTipoCode", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportAlturasLosaQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportAlturasLosaQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.AlturasLosa.Query().AsNoTracking();

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

            var proyeccion = sortedQuery.Select(t => new AlturaLosaResponse
            {
                Code = t.Code,
                Name = t.Name,
                Value = t.Value,
                Width = t.Width,
                ProductoTipoCode = t.ProductoTipoCode,
                ProductoCode = t.ProductoCode,
                PolystyreneProductoTipoCode = t.PolystyreneProductoTipoCode,
                PolystyreneProductoCode = t.PolystyreneProductoCode,
                PolystyreneValue = t.PolystyreneValue,
                PolystyreneWidth = t.PolystyreneWidth,
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
                "Alturas de losa", "alturas_losa", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
