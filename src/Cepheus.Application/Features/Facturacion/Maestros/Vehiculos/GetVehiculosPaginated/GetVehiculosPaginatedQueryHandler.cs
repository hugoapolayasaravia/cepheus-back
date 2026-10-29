using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common;
using Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.CreateVehiculo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.GetVehiculosPaginated
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

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(v => v.TransportistaCode).ThenBy(v => v.VehicleType).ThenBy(v => v.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo)
            // para no depender de traducir el enum a texto en SQL.
            var pagedEntities = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<VehiculoResponse>
            {
                Items = pagedEntities.Items.Select(CreateVehiculoCommandHandler.Map).ToList(),
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
        }
    }
}
