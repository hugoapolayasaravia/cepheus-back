using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ReopenPeriodoContable
{
    public record ReopenPeriodoContableCommand(string Code) : IRequest<PeriodoContableResponse>;
}
