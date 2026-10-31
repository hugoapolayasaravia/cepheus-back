using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.CreateVehiculo;
using Cepheus.Application.Features.Facturacion.Maestros.Vendedores.CreateVendedor;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.ExportVehiculos
{
    public class ExportVehiculosQueryHandler : IRequestHandler<ExportVehiculosQuery, ExportFileResponse>
    {
        private static readonly IReadOnlyList<ExportColumn<VehiculoResponse>> Catalogo = new List<ExportColumn<VehiculoResponse>>
        {
            new("transportistaCode", "Transportista", f => f.TransportistaCode),
            new("vehicleType", "Tipo de vehículo", f => f.VehicleType),
            new("code", "Código", f => f.Code),
            new("licensePlate", "Placa", f => f.LicensePlate),
            new("brand", "Marca", f => f.Brand),
            new("model", "Modelo", f => f.Model),
            new("choferCode", "Chofer", f => f.ChoferCode),
            new("capacity", "Capacidad", f => f.Capacity),
            new("suple", "Suple", f => f.Suple),
            new("lengthM", "Largo (m)", f => f.LengthM),
            new("widthM", "Ancho (m)", f => f.WidthM),
            new("heightM", "Alto (m)", f => f.HeightM),
            new("telescopic", "Telescópico", f => f.Telescopic),
            new("withoutSuple", "Sin suple", f => f.WithoutSuple),
            new("withSuple", "Con suple", f => f.WithSuple),
            new("cubicWithoutSuple", "Cúbico sin suple", f => f.CubicWithoutSuple),
            new("cubicWithSuple", "Cúbico con suple", f => f.CubicWithSuple),
            new("mtcInternalCode", "Código interno MTC", f => f.MtcInternalCode),
            new("vehicularConfiguration", "Configuración vehicular", f => f.VehicularConfiguration),
            new("planillaCode", "Planilla", f => f.PlanillaCode),
            new("observations", "Observaciones", f => f.Observations),
            new("approvedBy", "Aprobado por", f => f.ApprovedBy),
            new("approvedAt", "Aprobado el", f => HoraLocal.Convertir(f.ApprovedAt)),
            new("isActive", "Estado", f => ExportBuilder.Estado(f.IsActive)),
            new("createdAt", "Creado", f => HoraLocal.Convertir(f.CreatedAt)),
            new("updatedAt", "Modificado", f => HoraLocal.Convertir(f.UpdatedAt)),
            new("createdBy", "Creado por", f => f.CreatedBy),
            new("updatedBy", "Modificado por", f => f.UpdatedBy),
        };

        private static readonly string[] PorDefecto = { "transportistaCode", "vehicleType", "code", "licensePlate", "brand", "model", "choferCode", "isActive" };

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

            var query = _uow.Facturacion.Maestros.Vehiculos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(v =>
                    v.Code.ToLower().Contains(search) ||
                    v.LicensePlate.ToLower().Contains(search) ||
                    (v.Brand != null && v.Brand.ToLower().Contains(search)) ||
                    (v.Model != null && v.Model.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.TransportistaCode))
            {
                var transportistaCode = request.TransportistaCode.Trim().ToUpper();
                query = query.Where(v => v.TransportistaCode == transportistaCode);
            }

            if (TipoVehiculoParser.TryParse(request.VehicleType, out var vehicleType))
            {
                query = query.Where(v => v.VehicleType == vehicleType);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == request.IsActive.Value);
            }

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(v => v.TransportistaCode).ThenBy(v => v.VehicleType).ThenBy(v => v.Code)
                : query.ApplySort(request.SortBy, (request.SortDesc ?? false));

            //var proyeccion = sortedQuery.Select(CreateVehiculoCommandHandler.Map);

            //var filas = await proyeccion
            //    .Take(ExportBuilder.MaxFilas(formato) + 1)
            //    .ToListAsync(cancellationToken);
            var entidades = await sortedQuery
                .Take(ExportBuilder.MaxFilas(formato) + 1)
                .ToListAsync(cancellationToken);

            var filas = entidades
                .Select(CreateVehiculoCommandHandler.Map)
                .ToList();


            return ExportBuilder.Build(
                _exportService, _reportSettings, _currentUser, formato,
                "Vehículos", "vehiculos", filas, Catalogo, request.Columns, PorDefecto,
                ExportBuilder.Filtros(request.Search, request.IsActive,
                    string.IsNullOrWhiteSpace(request.TransportistaCode) ? null : $"Transportista: {request.TransportistaCode.Trim()}",
                    string.IsNullOrWhiteSpace(request.VehicleType) ? null : $"Tipo de vehículo: {request.VehicleType.Trim()}"));
        }
    }
}
