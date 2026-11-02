using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.ExportConductores
{
    public class ExportConductoresQueryHandler : IRequestHandler<ExportConductoresQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<ConductorResponse>> Catalogo = new List<ExportColumn<ConductorResponse>>
        {
            new("code", "Código", f => f.Code),
            new("documentTypeCode", "Tipo de documento", f => f.DocumentTypeCode),
            new("documentNumber", "Número de documento", f => f.DocumentNumber),
            new("firstName", "Nombres", f => f.FirstName),
            new("lastName", "Apellidos", f => f.LastName),
            new("driverLicenseNumber", "Licencia", f => f.DriverLicenseNumber),
            new("licenseCategory", "Categoría", f => f.LicenseCategory),
            new("phone", "Teléfono", f => f.Phone),
            new("email", "Correo", f => f.Email),
            new("observations", "Observaciones", f => f.Observations),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "documentNumber", "firstName", "lastName", "driverLicenseNumber", "licenseCategory", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportConductoresQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportConductoresQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Logistica.Maestros.Conductores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c => c.Code.ToLower().Contains(search) || c.DocumentNumber.ToLower().Contains(search) || c.FirstName.ToLower().Contains(search) || c.LastName.ToLower().Contains(search) || c.DriverLicenseNumber.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(c => new ConductorResponse
                {
                Code = c.Code,
                DocumentTypeCode = c.DocumentTypeCode,
                DocumentNumber = c.DocumentNumber,
                FirstName = c.FirstName,
                LastName = c.LastName,
                DriverLicenseNumber = c.DriverLicenseNumber,
                LicenseCategory = c.LicenseCategory,
                Phone = c.Phone,
                Email = c.Email,
                Observations = c.Observations,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                RowVersion = c.RowVersion
                })
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Conductores", "conductores", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive));
        }
    }
}
