using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.CreateCentroCosto
{
    public record CreateCentroCostoCommand(
        string Name,
        string PlantaCode
    ) : IRequest<CentroCostoResponse>;
}
