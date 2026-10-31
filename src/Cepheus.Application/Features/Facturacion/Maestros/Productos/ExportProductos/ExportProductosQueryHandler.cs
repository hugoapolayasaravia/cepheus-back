using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.CreateProducto;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.ExportProductos
{
    public class ExportProductosQueryHandler : IRequestHandler<ExportProductosQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ProductoResponse>> Catalogo = new List<ExportColumn<ProductoResponse>>
        {
            new("tipoProductoCode", "Tipo de producto", f => f.TipoProductoCode),
            new("code", "Código", f => f.Code),
            new("fullCode", "Código completo", f => f.FullCode),
            new("name", "Nombre", f => f.Name),
            new("shortName", "Nombre corto", f => f.ShortName),
            new("unitCode", "Unidad", f => f.UnitCode),
            new("accountingAccountCode", "Cuenta contable", f => f.AccountingAccountCode),
            new("transportAccountCode", "Cuenta de transporte", f => f.TransportAccountCode),
            new("creditNoteAccountCode", "Cuenta de nota de crédito", f => f.CreditNoteAccountCode),
            new("lengthLimit", "Límite de longitud", f => f.LengthLimit),
            new("transportTipoProductoCode", "Tipo de producto (transporte)", f => f.TransportTipoProductoCode),
            new("transportCode", "Producto de transporte", f => f.TransportCode),
            new("categoryCode", "Categoría", f => f.CategoryCode),
            new("strengthCode", "Resistencia", f => f.StrengthCode),
            new("cementTypeCode", "Tipo de cemento", f => f.CementTypeCode),
            new("stoneSizeCode", "Tamaño de piedra", f => f.StoneSizeCode),
            new("slumpCode", "Slump", f => f.SlumpCode),
            new("waterCementRatioCode", "Relación agua/cemento", f => f.WaterCementRatioCode),
            new("ageCode", "Edad", f => f.AgeCode),
            new("specialConditionCode", "Condición especial", f => f.SpecialConditionCode),
            new("mixProportionCode", "Proporción de mezcla", f => f.MixProportionCode),
            new("isPumpable", "Bombeable", f => ExportBuilder.SiNo(f.IsPumpable)),
            new("isSubjectToDetraction", "Sujeto a detracción", f => ExportBuilder.SiNo(f.IsSubjectToDetraction)),
            new("goodsTypeCode", "Tipo de bien", f => f.GoodsTypeCode),
            new("operationTypeCode", "Tipo de operación", f => f.OperationTypeCode),
            new("cementValue", "Valor de cemento", f => f.CementValue),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "tipoProductoCode", "code", "fullCode", "name", "shortName", "unitCode", "accountingAccountCode", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportProductosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportProductosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Maestros.Productos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p =>
                    (p.TipoProductoCode + p.Code).ToLower().Contains(search) ||
                    p.Name.ToLower().Contains(search) ||
                    (p.ShortName != null && p.ShortName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoProductoCode))
            {
                var tipoProductoCode = request.TipoProductoCode.Trim().ToUpper();
                query = query.Where(p => p.TipoProductoCode == tipoProductoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.CategoryCode))
            {
                var categoryCode = request.CategoryCode.Trim().ToUpper();
                query = query.Where(p => p.CategoryCode == categoryCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.TipoProductoCode).ThenBy(p => p.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateProductoCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);

            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateProductoCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Productos", "productos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.TipoProductoCode) ? null : $"Tipo de producto: {request.TipoProductoCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.CategoryCode) ? null : $"Categoría: {request.CategoryCode.Trim()}"));
        }
    }
}
