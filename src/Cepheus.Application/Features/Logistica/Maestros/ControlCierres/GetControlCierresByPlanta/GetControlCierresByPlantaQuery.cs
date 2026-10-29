using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.GetControlCierresByPlanta
{
    public record GetControlCierresByPlantaQuery(string PlantaCode) : IRequest<List<ControlCierreResponse>>;
}
