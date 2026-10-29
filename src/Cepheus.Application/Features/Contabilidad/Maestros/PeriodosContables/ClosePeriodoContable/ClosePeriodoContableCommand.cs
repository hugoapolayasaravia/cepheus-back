using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ClosePeriodoContable
{
    public record ClosePeriodoContableCommand(string Code) : IRequest<PeriodoContableResponse>;
}
