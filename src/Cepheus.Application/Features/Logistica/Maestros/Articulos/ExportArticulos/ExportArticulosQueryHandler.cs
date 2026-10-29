using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.ExportArticulos
{
    public class ExportArticulosQueryHandler : IRequestHandler<ExportArticulosQuery, ExportFileResponse>
    {
        private sealed record Fila(string Code, string Name, string UnidadMedidaCode, string? UnidadMedidaName, string SubFamiliaCode, string? SubFamiliaName, decimal MinStock, decimal MaxStock, decimal IncomingStock, decimal LeadTimeDays, AbcClass AbcClass, string TipoArticuloCode, string PlanCode, string? ManufacturerCode, bool IsAgreement, string? PlantOriginCode, bool IsActive, DateTime CreatedAt, string? CreatedBy, DateTime? UpdatedAt, string? UpdatedBy);

        private static readonly IReadOnlyList<ExportColumn<Fila>> Catalogo = new List<ExportColumn<Fila>>
        {
            new("code", "Código", f => f.Code),
            new("name", "Descripción", f => f.Name),
            new("unidadMedidaCode", "UM", f => f.UnidadMedidaCode),
            new("unidadMedidaName", "Unidad de medida", f => f.UnidadMedidaName),
            new("subFamiliaCode", "Subfamilia", f => f.SubFamiliaCode),
            new("subFamiliaName", "Nombre subfamilia", f => f.SubFamiliaName),
            new("minStock", "Stock mínimo", f => f.MinStock),
            new("maxStock", "Stock máximo", f => f.MaxStock),
            new("incomingStock", "Stock de entrada", f => f.IncomingStock),
            new("leadTimeDays", "Lead time (días)", f => f.LeadTimeDays),
            new("abcClass", "ABC", f => f.AbcClass.ToString()),
            new("tipoArticuloCode", "Tipo", f => f.TipoArticuloCode),
            new("planCode", "Plan", f => f.PlanCode),
            new("manufacturerCode", "Fabricante", f => f.ManufacturerCode),
            new("isAgreement", "Convenio", f => ExportBuilder.SiNo(f.IsAgreement)),
            new("plantOriginCode", "Planta origen", f => f.PlantOriginCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "name", "unidadMedidaCode", "subFamiliaCode", "subFamiliaName", "abcClass", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportArticulosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportArticulosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Logistica.Maestros.Articulos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x => x.Code.ToLower().Contains(search) || x.Name.ToLower().Contains(search) || (x.ManufacturerCode != null && x.ManufacturerCode.ToLower().Contains(search)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SubFamiliaCode))
            {
                query = query.Where(x => x.SubFamiliaCode == request.SubFamiliaCode!.Trim().ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(request.TipoArticuloCode))
            {
                query = query.Where(x => x.TipoArticuloCode == request.TipoArticuloCode!.Trim().ToUpper());
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(x => new Fila(x.Code, x.Name, x.UnidadMedidaCode, x.UnidadMedida.Name, x.SubFamiliaCode, x.SubFamilia.Name, x.MinStock, x.MaxStock, x.IncomingStock, x.LeadTimeDays, x.AbcClass, x.TipoArticuloCode, x.PlanCode, x.ManufacturerCode, x.IsAgreement, x.PlantOriginCode, x.IsActive, x.CreatedAt, x.CreatedBy, x.UpdatedAt, x.UpdatedBy))
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Artículos", "articulos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                !string.IsNullOrWhiteSpace(request.SubFamiliaCode) ? $"Subfamilia: {request.SubFamiliaCode}" : null,
                !string.IsNullOrWhiteSpace(request.TipoArticuloCode) ? $"Tipo: {request.TipoArticuloCode}" : null));
        }
    }
}
