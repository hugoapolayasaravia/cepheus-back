using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.GetPlanesPreventivoPaginated
{
    public class GetPlanesPreventivoPaginatedQueryHandler
        : IRequestHandler<GetPlanesPreventivoPaginatedQuery, PagedResult<PlanPreventivoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPlanesPreventivoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PlanPreventivoResponse>> Handle(
            GetPlanesPreventivoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = request.PlantaCode.Trim().ToUpper();
                query = query.Where(x => x.PlantaCode == planta);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim().ToUpper();
                query = query.Where(x => x.Code == code);
            }

            if (!string.IsNullOrWhiteSpace(request.EquipoCode))
            {
                var equipo = request.EquipoCode.Trim().ToUpper();
                query = query.Where(x => x.EquipoCode.Contains(equipo));
            }

            if (!string.IsNullOrWhiteSpace(request.ResponsableCode))
            {
                var responsable = request.ResponsableCode.Trim().ToUpper();
                query = query.Where(x => x.ResponsableCode == responsable);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Description.ToLower().Contains(search) ||
                    x.Equipo.Name.ToLower().Contains(search));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            // Legacy: ORDER BY Codigo_Pla DESC, Codigo_Mpv DESC
            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.PlantaCode).ThenByDescending(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(PlanPreventivoProjection.ToResponse);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
