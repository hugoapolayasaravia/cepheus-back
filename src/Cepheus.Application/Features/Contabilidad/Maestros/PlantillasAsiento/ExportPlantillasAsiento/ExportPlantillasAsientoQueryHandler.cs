using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.ExportPlantillasAsiento
{
    public class ExportPlantillasAsientoQueryHandler : IRequestHandler<ExportPlantillasAsientoQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string Name, ProcesoAsiento Process, string TipoAsientoCode, string? PurchaseAccountCode, string? ComprobantePagoCode, string? DescriptionText, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("process", "Proceso", f => f.Process.ToString()),
            new("tipoAsientoCode", "Tipo de asiento", f => f.TipoAsientoCode),
            new("purchaseAccountCode", "Cuenta de compra", f => f.PurchaseAccountCode),
            new("comprobantePagoCode", "Comprobante de pago", f => f.ComprobantePagoCode),
            new("descriptionText", "Texto de descripción", f => f.DescriptionText),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "code", "name", "process", "tipoAsientoCode", "purchaseAccountCode", "comprobantePagoCode", "descriptionText", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPlantillasAsientoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPlantillasAsientoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;

            var query = _uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking();

            IQueryable<Cepheus.Domain.Contabilidad.Maestros.PlantillaAsiento> filtered = query;

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                filtered = filtered.Where(p => p.Code.Contains(search) || p.Name.Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                filtered = filtered.Where(p => p.IsActive == request.IsActive.Value);
            }

            if (request.Process.HasValue)
            {
                filtered = filtered.Where(p => p.Process == request.Process.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoAsientoCode))
            {
                var tipo = request.TipoAsientoCode.Trim().ToUpper();
                filtered = filtered.Where(p => p.TipoAsientoCode == tipo);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? filtered.OrderBy(p => p.Code)
                : filtered.ApplySort(request.SortBy, (request.SortDesc ?? false));

            var proyeccion = sortedQuery.Select(p => new Fila(p.Code, p.Name, p.Process, p.TipoAsientoCode, p.PurchaseAccountCode, p.ComprobantePagoCode, p.DescriptionText, p.IsActive, p.CreatedAt, p.UpdatedAt));

            var filas = await proyeccion
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Plantillas de asiento", "plantillas_asiento", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    request.Process.HasValue ? $"Proceso: {request.Process.Value}" : null,
                    string.IsNullOrWhiteSpace(request.TipoAsientoCode) ? null : $"Tipo de asiento: {request.TipoAsientoCode.Trim()}"));
        }
    }
}
