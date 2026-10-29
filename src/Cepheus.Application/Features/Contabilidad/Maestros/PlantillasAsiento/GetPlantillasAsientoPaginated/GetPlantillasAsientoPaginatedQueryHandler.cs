using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.GetPlantillasAsientoPaginated
{
    public class GetPlantillasAsientoPaginatedQueryHandler
        : IRequestHandler<GetPlantillasAsientoPaginatedQuery, PagedResult<PlantillaAsientoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPlantillasAsientoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PlantillaAsientoResponse>> Handle(
            GetPlantillasAsientoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines);

            IQueryable<Cepheus.Domain.Contabilidad.Maestros.PlantillaAsiento> filtered = query;

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                filtered = filtered.Where(p => p.Code.Contains(search) || p.Name.Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                filtered = filtered.Where(p => p.IsActive == request.IsActive.Value);
            }

            if (request.Process.HasValue)
            {
                filtered = filtered.Where(p => p.Process == request.Process.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.TipoAsientoCode))
            {
                var tipo = request.TipoAsientoCode.Trim().ToUpper();
                filtered = filtered.Where(p => p.TipoAsientoCode == tipo);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? filtered.OrderBy(p => p.Code)
                : filtered.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<PlantillaAsientoResponse>
            {
                Items = paged.Items.Select(PlantillaAsientoMapper.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
