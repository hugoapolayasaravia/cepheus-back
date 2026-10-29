using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodosContablesPaginated
{
    public class GetPeriodosContablesPaginatedQueryHandler
        : IRequestHandler<GetPeriodosContablesPaginatedQuery, PagedResult<PeriodoContableResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPeriodosContablesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PeriodoContableResponse>> Handle(
            GetPeriodosContablesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Maestros.PeriodosContables.Query().AsNoTracking();

            if (request.Year.HasValue)
            {
                query = query.Where(x => x.Year == request.Year.Value);
            }

            if (request.Month.HasValue)
            {
                query = query.Where(x => x.Month == request.Month.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 12;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<PeriodoContableResponse>
            {
                Items = paged.Items.Select(PeriodoContableMapper.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
