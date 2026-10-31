using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.ExportStockProductos
{
    public class ExportStockProductosQueryHandler : IRequestHandler<ExportStockProductosQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<StockProductoResponse>> Catalogo = new List<ExportColumn<StockProductoResponse>>
        {
            new("id", "Id", f => f.Id),
            new("plantaCode", "Planta", f => f.PlantaCode),
            new("tipoProductoCode", "Tipo de producto", f => f.TipoProductoCode),
            new("productoCode", "Producto", f => f.ProductoCode),
            new("cantidad", "Cantidad", f => f.Cantidad),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "id", "plantaCode", "tipoProductoCode", "productoCode", "cantidad" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportStockProductosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportStockProductosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.StockProductos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = request.PlantaCode.Trim().ToUpper();
                query = query.Where(x => x.PlantaCode == planta);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoProductoCode))
            {
                var tipo = request.TipoProductoCode.Trim().ToUpper();
                query = query.Where(x => x.TipoProductoCode == tipo);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoCode))
            {
                var code = request.ProductoCode.Trim().ToUpper();
                query = query.Where(x => x.ProductoCode == code);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.PlantaCode).ThenBy(x => x.TipoProductoCode).ThenBy(x => x.ProductoCode)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateStockProductoCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateStockProductoCommandHandler.Map)
                .ToList();


            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Stock de productos", "stock_productos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, null,
                    string.IsNullOrWhiteSpace(request.PlantaCode) ? null : $"Planta: {request.PlantaCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.TipoProductoCode) ? null : $"Tipo de producto: {request.TipoProductoCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.ProductoCode) ? null : $"Producto: {request.ProductoCode.Trim()}"));
        }
    }
}
