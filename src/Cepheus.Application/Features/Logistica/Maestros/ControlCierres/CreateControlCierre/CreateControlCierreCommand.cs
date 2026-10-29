using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.CreateControlCierre
{
    public record CreateControlCierreCommand(
        string PlantaCode,
        string PeriodCode,
        DateTime ClosureDate,
        decimal DifferenceAmount
    ) : IRequest<ControlCierreResponse>;
}
