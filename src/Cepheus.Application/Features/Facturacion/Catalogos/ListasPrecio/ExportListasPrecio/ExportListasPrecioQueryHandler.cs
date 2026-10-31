using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.ExportListasPrecio
{
    public class ExportListasPrecioQueryHandler : IRequestHandler<ExportListasPrecioQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ListaPrecioResponse>> Catalogo = new List<ExportColumn<ListaPrecioResponse>>
        {
            new("id", "Id", f => f.Id),
            new("tipoProductoCode", "Tipo de producto", f => f.TipoProductoCode),
            new("productoCode", "Producto", f => f.ProductoCode),
            new("precio", "Precio", f => f.Precio),
            new("fechaInicio", "Inicio", f => f.FechaInicio.ToString("dd/MM/yyyy")),
            new("fechaFin", "Fin", f => f.FechaFin?.ToString("dd/MM/yyyy")),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "id", "tipoProductoCode", "productoCode", "precio", "fechaInicio", "fechaFin", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportListasPrecioQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportListasPrecioQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Facturacion.Catalogos.ListasPrecio.Query().AsNoTracking();

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

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (request.Vigente == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.FechaInicio <= now && (x.FechaFin == null || x.FechaFin >= now));
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.FechaInicio)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateListaPrecioCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateListaPrecioCommandHandler.Map)
                .ToList();

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Listas de precio", "listas_precio", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(null, request.IsActive,
                    string.IsNullOrWhiteSpace(request.TipoProductoCode) ? null : $"Tipo de producto: {request.TipoProductoCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.ProductoCode) ? null : $"Producto: {request.ProductoCode.Trim()}",
                    request.Vigente.HasValue ? $"Solo vigentes: {ExportBuilder.SiNo(request.Vigente.Value)}" : null));
        }
    }
}
