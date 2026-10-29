using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.GetUnidadesParadaPaginated
{
    public class GetUnidadesParadaPaginatedQueryHandler
        : IRequestHandler<GetUnidadesParadaPaginatedQuery, PagedResult<UnidadParadaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadesParadaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UnidadParadaResponse>> Handle(
            GetUnidadesParadaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Transacciones.UnidadesParada.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SubCentroCostoCode))
            {
                var scc = request.SubCentroCostoCode.Trim().ToUpper();
                query = query.Where(x => x.SubCentroCostoCode == scc);
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaAfectadaCode))
            {
                var plt = request.PlantaAfectadaCode.Trim().ToUpper();
                query = query.Where(x => x.PlantaAfectadaCode == plt);
            }

            if (!string.IsNullOrWhiteSpace(request.FallaCode))
            {
                var fk = request.FallaCode.Trim().ToUpper();
                query = query.Where(x => x.FallaCode == fk);
            }

            if (request.Estado.HasValue)
            {
                query = query.Where(x => x.Estado == request.Estado.Value);
            }

            if (request.FechaDesde.HasValue)
            {
                var desde = request.FechaDesde.Value.Date;
                query = query.Where(x => x.Fecha >= desde);
            }

            if (request.FechaHasta.HasValue)
            {
                var hasta = request.FechaHasta.Value.Date;
                query = query.Where(x => x.Fecha <= hasta);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.SubCentroCostoCode.ToLower().Contains(search) ||
                    x.SubCentroCosto.Name.ToLower().Contains(search));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.Fecha).ThenBy(x => x.SubCentroCostoCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(UnidadParadaProjection.ToResponse);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
