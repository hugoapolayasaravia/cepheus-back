using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.ExportPreciosProducto
{
    public class ExportPreciosProductoQueryHandler : IRequestHandler<ExportPreciosProductoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<PrecioProductoResponse>> Catalogo = new List<ExportColumn<PrecioProductoResponse>>
        {
            new("fleteCode", "Flete", f => f.FleteCode),
            new("productoTipoCode", "Tipo de producto", f => f.ProductoTipoCode),
            new("productoCode", "Producto", f => f.ProductoCode),
            new("currencyTypeCode", "Tipo de moneda", f => f.CurrencyTypeCode),
            new("currencyCode", "Moneda", f => f.CurrencyCode),
            new("amount", "Importe", f => f.Amount),
            new("transportAmount", "Importe de transporte", f => f.TransportAmount),
            new("freightAmount", "Importe de flete", f => f.FreightAmount),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "fleteCode", "productoTipoCode", "productoCode", "currencyTypeCode", "currencyCode", "amount", "transportAmount", "freightAmount" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPreciosProductoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPreciosProductoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.PreciosProducto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.FleteCode))
            {
                query = query.Where(p => p.FleteCode == request.FleteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoTipoCode))
            {
                query = query.Where(p => p.ProductoTipoCode == request.ProductoTipoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ProductoCode))
            {
                query = query.Where(p => p.ProductoCode == request.ProductoCode);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.FleteCode).ThenBy(p => p.ProductoTipoCode).ThenBy(p => p.ProductoCode)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(p => new PrecioProductoResponse
            {
                FleteCode = p.FleteCode,
                ProductoTipoCode = p.ProductoTipoCode,
                ProductoCode = p.ProductoCode,
                CurrencyTypeCode = p.CurrencyTypeCode,
                CurrencyCode = p.CurrencyCode,
                Amount = p.Amount,
                TransportAmount = p.TransportAmount,
                FreightAmount = p.FreightAmount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CreatedBy = p.CreatedBy,
                UpdatedBy = p.UpdatedBy,
                RowVersion = p.RowVersion
            });

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Precios de producto", "precios_producto", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, null,
                    string.IsNullOrWhiteSpace(request.FleteCode) ? null : $"Flete: {request.FleteCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.ProductoTipoCode) ? null : $"Tipo de producto: {request.ProductoTipoCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.ProductoCode) ? null : $"Producto: {request.ProductoCode.Trim()}"));
        }
    }
}
