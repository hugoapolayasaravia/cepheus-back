using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.ExportComprobantesPago
{
    public class ExportComprobantesPagoQueryHandler : IRequestHandler<ExportComprobantesPagoQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string SunatCode, string Name, string ShortName, string? Description, bool RequiresRuc, bool RequiresAddress, bool AffectsIgv, bool IsNonTaxable, bool AffectsIncomeTax, bool AffectsFonavi, bool IsService, bool AffectsForeignIgv, bool AvailableForPurchaseOrder, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("sunatCode", "Cód. SUNAT", f => f.SunatCode),
            new("name", "Nombre", f => f.Name),
            new("shortName", "Abreviatura", f => f.ShortName),
            new("description", "Descripción", f => f.Description),
            new("requiresRuc", "Exige RUC", f => ExportBuilder.SiNo(f.RequiresRuc)),
            new("requiresAddress", "Exige dirección", f => ExportBuilder.SiNo(f.RequiresAddress)),
            new("affectsIgv", "Afecta IGV", f => ExportBuilder.SiNo(f.AffectsIgv)),
            new("isNonTaxable", "Inafecto", f => ExportBuilder.SiNo(f.IsNonTaxable)),
            new("affectsIncomeTax", "Afecta renta", f => ExportBuilder.SiNo(f.AffectsIncomeTax)),
            new("affectsFonavi", "Afecta FONAVI", f => ExportBuilder.SiNo(f.AffectsFonavi)),
            new("isService", "Servicio", f => ExportBuilder.SiNo(f.IsService)),
            new("affectsForeignIgv", "IGV exterior", f => ExportBuilder.SiNo(f.AffectsForeignIgv)),
            new("availableForPurchaseOrder", "Orden de compra", f => ExportBuilder.SiNo(f.AvailableForPurchaseOrder)),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "code", "sunatCode", "name", "shortName", "requiresRuc", "affectsIgv", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportComprobantesPagoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportComprobantesPagoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Comunes.ComprobantesPago.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x => x.Code.ToLower().Contains(search) || x.SunatCode.ToLower().Contains(search) || x.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Name)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(x => new Fila(x.Code, x.SunatCode, x.Name, x.ShortName, x.Description, x.RequiresRuc, x.RequiresAddress, x.AffectsIgv, x.IsNonTaxable, x.AffectsIncomeTax, x.AffectsFonavi, x.IsService, x.AffectsForeignIgv, x.AvailableForPurchaseOrder, x.IsActive, x.CreatedAt, x.UpdatedAt))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Comprobantes de pago", "comprobantes_pago", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
