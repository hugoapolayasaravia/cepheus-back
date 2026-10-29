using Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.UpdateCentroCosto
{
    public record UpdateCentroCostoCommand(
        string Code,
        string Name,
        string PlantaCode,
        byte[] RowVersion
    ) : IRequest<CentroCostoResponse>;
}
