using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.ExportMotivosDevolucion
{
    public class ExportMotivosDevolucionQueryHandler : IRequestHandler<ExportMotivosDevolucionQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string Name, bool AffectsStock, bool EsVenta, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("affectsStock", "Afecta stock", f => ExportBuilder.SiNo(f.AffectsStock)),
            new("esVenta", "Es de venta", f => ExportBuilder.SiNo(f.EsVenta)),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "code", "name", "affectsStock", "esVenta", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportMotivosDevolucionQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportMotivosDevolucionQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Comunes.MotivosDevolucion.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x => x.Code.ToLower().Contains(search) || x.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (request.AffectsStock.HasValue)
            {
                query = query.Where(x => x.AffectsStock == request.AffectsStock.Value);
            }

            if (request.EsVenta.HasValue)
            {
                query = query.Where(x => x.EsVenta == request.EsVenta.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Name)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(x => new Fila(x.Code, x.Name, x.AffectsStock, x.EsVenta, x.IsActive, x.CreatedAt, x.UpdatedAt))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Motivos de devolución", "motivos_devolucion", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                request.AffectsStock.HasValue ? $"Afecta stock: {(request.AffectsStock!.Value ? "Sí" : "No")}" : null,
                request.EsVenta.HasValue ? $"Es de venta: {(request.EsVenta!.Value ? "Sí" : "No")}" : null));
        }
    }
}
