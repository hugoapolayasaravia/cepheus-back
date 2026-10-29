using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodosIgvPaginated;

public class GetPeriodosIgvPaginatedQueryHandler
    : IRequestHandler<GetPeriodosIgvPaginatedQuery, PagedResult<PeriodoIgvResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetPeriodosIgvPaginatedQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<PeriodoIgvResponse>> Handle(
        GetPeriodosIgvPaginatedQuery request, CancellationToken ct)
    {
        var query = _uow.Comunes.PeriodosIgv.Query().AsNoTracking();

        if (request.FromDate.HasValue)
        {
            var desde = request.FromDate.Value;
            query = query.Where(p => p.FechaFin == null || p.FechaFin >= desde);
        }

        if (request.ToDate.HasValue)
        {
            var hasta = request.ToDate.Value;
            query = query.Where(p => p.FechaInicio <= hasta);
        }

        var sortDesc = request.SortDesc ?? true;
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;

        var sorted = string.IsNullOrWhiteSpace(request.SortBy)
            ? query.OrderByDescending(p => p.FechaInicio)
            : query.ApplySort(request.SortBy, sortDesc);

        var projected = sorted.Select(p => new PeriodoIgvResponse
        {
            Id = p.Id,
            FechaInicio = p.FechaInicio,
            FechaFin = p.FechaFin,
            PorcentajeIgv = p.PorcentajeIgv,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedBy = p.UpdatedBy,
            RowVersion = p.RowVersion
        });

        return await projected.ToPagedResultAsync(pageNumber, pageSize, ct);
    }
}
