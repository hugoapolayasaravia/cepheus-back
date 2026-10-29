using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Plantas.ExportPlantas
{
    public class ExportPlantasQueryHandler : IRequestHandler<ExportPlantasQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string Name, string? LegalName, string Address, string? UbigeoCode, string? ManagerName, bool HasWarehouse, bool IsProductionPlant, bool IsProject, bool RequiresApprovals, bool AppliesDetraction, string? StatusCode, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Nombre", f => f.Name),
            new("legalName", "Razón social", f => f.LegalName),
            new("address", "Dirección", f => f.Address),
            new("ubigeoCode", "Ubigeo", f => f.UbigeoCode),
            new("managerName", "Responsable", f => f.ManagerName),
            new("hasWarehouse", "Almacén", f => ExportBuilder.SiNo(f.HasWarehouse)),
            new("isProductionPlant", "Producción", f => ExportBuilder.SiNo(f.IsProductionPlant)),
            new("isProject", "Proyecto", f => ExportBuilder.SiNo(f.IsProject)),
            new("requiresApprovals", "Aprobaciones", f => ExportBuilder.SiNo(f.RequiresApprovals)),
            new("appliesDetraction", "Detracción", f => ExportBuilder.SiNo(f.AppliesDetraction)),
            new("statusCode", "Estado (cód.)", f => f.StatusCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
        };

        private static readonly string[] PorDefecto = { "code", "name", "address", "ubigeoCode", "managerName", "hasWarehouse", "isProductionPlant", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportPlantasQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportPlantasQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Comunes.Plantas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x => x.Code.ToLower().Contains(search) || x.Name.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (request.IsProductionPlant.HasValue)
            {
                query = query.Where(x => x.IsProductionPlant == request.IsProductionPlant.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Name)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(x => new Fila(x.Code, x.Name, x.LegalName, x.Address, x.UbigeoCode, x.ManagerName, x.HasWarehouse, x.IsProductionPlant, x.IsProject, x.RequiresApprovals, x.AppliesDetraction, x.StatusCode, x.IsActive, x.CreatedAt, x.UpdatedAt))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Plantas", "plantas", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                request.IsProductionPlant.HasValue ? $"Planta de producción: {(request.IsProductionPlant!.Value ? "Sí" : "No")}" : null));
        }
    }
}
