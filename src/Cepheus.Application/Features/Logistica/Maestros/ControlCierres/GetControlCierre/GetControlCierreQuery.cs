using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.GetControlCierre
{
    public record GetControlCierreQuery(string PlantaCode, string PeriodCode) : IRequest<ControlCierreResponse>;
}
