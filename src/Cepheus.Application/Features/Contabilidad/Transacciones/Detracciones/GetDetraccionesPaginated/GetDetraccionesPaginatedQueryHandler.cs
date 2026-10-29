using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionesPaginated
{
    public class GetDetraccionesPaginatedQueryHandler
        : IRequestHandler<GetDetraccionesPaginatedQuery, PagedResult<DetraccionListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetDetraccionesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<DetraccionListItemResponse>> Handle(
            GetDetraccionesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Transacciones.Detracciones.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Year))
            {
                var year = request.Year.Trim();
                query = query.Where(d => d.Year == year);
            }

            if (!string.IsNullOrWhiteSpace(request.Period))
            {
                var period = request.Period.Trim();
                query = query.Where(d => d.Period == period);
            }

            if (!string.IsNullOrWhiteSpace(request.LotNumber))
            {
                var lot = request.LotNumber.Trim();
                query = query.Where(d => d.LotNumber == lot);
            }

            if (request.ProcessDate.HasValue)
            {
                // CreatedAt se guarda en UTC: se compara el día completo en hora de Perú.
                var from = DetraccionRules.LocalDayStartUtc(request.ProcessDate.Value);
                var to = DetraccionRules.LocalDayStartUtc(request.ProcessDate.Value.Date.AddDays(1));
                query = query.Where(d => d.CreatedAt >= from && d.CreatedAt < to);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!DetraccionRules.TryParseStatus(request.Status, out var status))
                {
                    throw new InvalidOperationException($"El estado debe ser uno de: {DetraccionRules.StatusNames}.");
                }

                query = query.Where(d => d.Status == status);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(d => d.Year).ThenByDescending(d => d.Period).ThenByDescending(d => d.LotNumber)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(DetraccionMapper.ToListItem);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
