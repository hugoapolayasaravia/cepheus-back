using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.ExportVehiculos
{
    public class ExportVehiculosQueryHandler : IRequestHandler<ExportVehiculosQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<VehiculoResponse>> Catalogo = new List<ExportColumn<VehiculoResponse>>
        {
            new("code", "Código", f => f.Code),
            new("transportistaCode", "Transportista", f => f.TransportistaCode),
            new("licensePlate", "Placa", f => f.LicensePlate),
            new("vehicleCategory", "Categoría", f => f.VehicleCategory),
            new("vehicleType", "Tipo", f => f.VehicleType),
            new("brand", "Marca", f => f.Brand),
            new("model", "Modelo", f => f.Model),
            new("manufactureYear", "Año", f => f.ManufactureYear),
            new("engineNumber", "Motor", f => f.EngineNumber),
            new("chassisNumber", "Chasis", f => f.ChassisNumber),
            new("color", "Color", f => f.Color),
            new("cargoCapacityKg", "Capacidad (kg)", f => f.CargoCapacityKg),
            new("lengthM", "Largo (m)", f => f.LengthM),
            new("widthM", "Ancho (m)", f => f.WidthM),
            new("heightM", "Alto (m)", f => f.HeightM),
            new("vehicularCertificateNumber", "Certificado vehicular", f => f.VehicularCertificateNumber),
            new("circulationCardNumber", "Tarjeta de circulación", f => f.CirculationCardNumber),
            new("vehicularConfiguration", "Configuración vehicular", f => f.VehicularConfiguration),
            new("observations", "Observaciones", f => f.Observations),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "code", "transportistaCode", "licensePlate", "brand", "model", "isActive" };

        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IReportSettings _reportSettings;
        private readonly ICurrentUserService _currentUser;

        public ExportVehiculosQueryHandler(
            IUnitOfWork uow, IExportService exportService,
            IReportSettings reportSettings, ICurrentUserService currentUser)
        {
            _uow = uow;
            _exportService = exportService;
            _reportSettings = reportSettings;
            _currentUser = currentUser;
        }

        public async Task<ExportFileResponse> Handle(ExportVehiculosQuery request, CancellationToken cancellationToken)
        {
            var formato = request.Format ?? ExportFormat.Excel;
            var query = _uow.Logistica.Maestros.Vehiculos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(v => v.Code.ToLower().Contains(search) || v.LicensePlate.ToLower().Contains(search) || (v.Brand != null && v.Brand.ToLower().Contains(search)) || (v.Model != null && v.Model.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.TransportistaCode))
            {
                var transportistaCode = request.TransportistaCode.Trim().ToUpper();
                query = query.Where(v => v.TransportistaCode == transportistaCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == request.IsActive.Value);
            }

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(v => v.Code)
                : query.ApplySort(request.SortBy, request.SortDesc ?? false);

            var filas = await sorted
                .Select(v => new VehiculoResponse
                {
                Code = v.Code,
                TransportistaCode = v.TransportistaCode,
                LicensePlate = v.LicensePlate,
                VehicleCategory = v.VehicleCategory,
                VehicleType = v.VehicleType,
                Brand = v.Brand,
                Model = v.Model,
                ManufactureYear = v.ManufactureYear,
                EngineNumber = v.EngineNumber,
                ChassisNumber = v.ChassisNumber,
                Color = v.Color,
                CargoCapacityKg = v.CargoCapacityKg,
                LengthM = v.LengthM,
                WidthM = v.WidthM,
                HeightM = v.HeightM,
                VehicularCertificateNumber = v.VehicularCertificateNumber,
                CirculationCardNumber = v.CirculationCardNumber,
                VehicularConfiguration = v.VehicularConfiguration,
                Observations = v.Observations,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                CreatedBy = v.CreatedBy,
                UpdatedBy = v.UpdatedBy,
                RowVersion = v.RowVersion
                })
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Vehículos", "vehiculos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                !string.IsNullOrWhiteSpace(request.TransportistaCode) ? $"Transportista: {request.TransportistaCode}" : null));
        }
    }
}
