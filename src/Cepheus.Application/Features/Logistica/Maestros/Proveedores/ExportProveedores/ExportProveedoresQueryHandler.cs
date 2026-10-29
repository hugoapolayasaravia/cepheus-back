using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.ExportProveedores
{
    public class ExportProveedoresQueryHandler : IRequestHandler<ExportProveedoresQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string DocumentTypeCode, string DocumentNumber, string LegalName, string? TradeName, ProviderType ProviderType, ProviderOrigin Origin, SunatCondition? SunatCondition, SunatTaxpayerStatus? SunatStatus, string? Observations, bool IsActive, DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("documentTypeCode", "Tipo doc.", f => f.DocumentTypeCode),
            new("documentNumber", "N° documento", f => f.DocumentNumber),
            new("legalName", "Razón social", f => f.LegalName),
            new("tradeName", "Nombre comercial", f => f.TradeName),
            new("providerType", "Tipo de proveedor", f => f.ProviderType.ToString()),
            new("origin", "Procedencia", f => f.Origin.ToString()),
            new("sunatCondition", "Condición SUNAT", f => f.SunatCondition.HasValue ? f.SunatCondition.Value.ToString() : null),
            new("sunatStatus", "Estado SUNAT", f => f.SunatStatus.HasValue ? f.SunatStatus.Value.ToString() : null),
            new("observations", "Observaciones", f => f.Observations),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "documentNumber", "legalName", "tradeName", "providerType", "origin", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportProveedoresQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportProveedoresQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Logistica.Maestros.Proveedores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x => x.Code.ToLower().Contains(search) || x.DocumentNumber.ToLower().Contains(search) || x.LegalName.ToLower().Contains(search) || (x.TradeName != null && x.TradeName.ToLower().Contains(search)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(x => new Fila(x.Code, x.DocumentTypeCode, x.DocumentNumber, x.LegalName, x.TradeName, x.ProviderType, x.Origin, x.SunatCondition, x.SunatStatus, x.Observations, x.IsActive, x.CreatedAt, x.CreatedBy, x.UpdatedAt, x.UpdatedBy))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Proveedores", "proveedores", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
