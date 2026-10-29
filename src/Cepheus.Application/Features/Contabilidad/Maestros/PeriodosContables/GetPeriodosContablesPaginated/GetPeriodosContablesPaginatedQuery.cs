using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodosContablesPaginated
{
    public class GetPeriodosContablesPaginatedQuery : PagedRequest, IRequest<PagedResult<PeriodoContableResponse>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public EstadoPeriodoContable? Status { get; set; }
    }
}
