using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.GetPeriodosIgvPaginated;

public class GetPeriodosIgvPaginatedQuery : PagedRequest, IRequest<PagedResult<PeriodoIgvResponse>>
{
    /// <summary>Si se indica, solo períodos que rigen en algún día desde esta fecha.</summary>
    public DateOnly? FromDate { get; set; }

    /// <summary>Si se indica, solo períodos que rigen en algún día hasta esta fecha.</summary>
    public DateOnly? ToDate { get; set; }
}
