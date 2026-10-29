using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.CreatePeriodoContable
{
    /// <summary>Registra (y deja Abierto) el período contable de un año y mes.</summary>
    public record CreatePeriodoContableCommand(int Year, int Month) : IRequest<PeriodoContableResponse>;
}
