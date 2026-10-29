using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.GetVehiculosPaginated
{
    public class GetVehiculosPaginatedQueryHandler
        : IRequestHandler<GetVehiculosPaginatedQuery, PagedResult<VehiculoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetVehiculosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<VehiculoResponse>> Handle(
            GetVehiculosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Maestros.Vehiculos.Query().AsNoTracking();

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

            if (request.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(v => v.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(v => new VehiculoResponse
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
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
