using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.GetPeriodoContableByCode
{
    public record GetPeriodoContableByCodeQuery(string Code) : IRequest<PeriodoContableResponse>;
}
