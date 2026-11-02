using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.ExportSubCentrosCosto
{
    public class ExportSubCentrosCostoQueryHandler : IRequestHandler<ExportSubCentrosCostoQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<SubCentroCostoResponse>> Catalogo = new List<ExportColumn<SubCentroCostoResponse>>
        {
            new("code", "Código", f => f.Code),
            new("centroCostoCode", "Centro de costo", f => f.CentroCostoCode),
            new("name", "Nombre", f => f.Name),
            new("accountingAccountCode", "Cuenta contable", f => f.AccountingAccountCode),
            new("accountingAttachmentTypeCode", "Tipo de anexo contable", f => f.AccountingAttachmentTypeCode),
            new("plantaCode", "Planta", f => f.PlantaCode),
            new("parentCode", "Subcentro padre", f => f.ParentCode),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "centroCostoCode", "name", "plantaCode", "parentCode", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportSubCentrosCostoQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportSubCentrosCostoQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Logistica.Maestros.SubCentrosCosto.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s => s.Code.ToLower().Contains(search) || s.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.CentroCostoCode))
            {
                var centroCostoCode = request.CentroCostoCode.Trim().ToUpper();
                query = query.Where(s => s.CentroCostoCode == centroCostoCode);
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(s => s.PlantaCode == plantaCode);
            }

            if (request.ParentCode is not null)
            {
                var parentCode = request.ParentCode.Trim().ToUpper();
                query = parentCode.Length == 0
                    ? query.Where(s => s.ParentCode == null)
                    : query.Where(s => s.ParentCode == parentCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(s => s.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(s => new SubCentroCostoResponse
                {
                Code = s.Code,
                CentroCostoCode = s.CentroCostoCode,
                Name = s.Name,
                AccountingAccountCode = s.AccountingAccountCode,
                AccountingAttachmentTypeCode = s.AccountingAttachmentTypeCode,
                PlantaCode = s.PlantaCode,
                ParentCode = s.ParentCode,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy,
                RowVersion = s.RowVersion
                })
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Subcentros de costo", "subcentros_costo", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                !string.IsNullOrWhiteSpace(request.CentroCostoCode) ? $"Centro de costo: {request.CentroCostoCode}" : null,
                !string.IsNullOrWhiteSpace(request.PlantaCode) ? $"Planta: {request.PlantaCode}" : null,
                request.ParentCode is not null ? (request.ParentCode.Trim().Length == 0 ? "Solo subcentros raíz" : $"Padre: {request.ParentCode}") : null));
        }
    }
}
