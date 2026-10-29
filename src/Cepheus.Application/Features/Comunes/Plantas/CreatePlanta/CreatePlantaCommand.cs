using Cepheus.Application.Features.Comunes.Plantas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.CreatePlanta
{
    public record CreatePlantaCommand(
        string Code,
        string Name,
        string? LegalName,
        string Address,
        string? AddressComplement,
        string? UbigeoCode,
        string? ManagerName,
        bool HasWarehouse,
        bool IsProductionPlant,
        bool IsProject,
        bool RequiresApprovals,
        bool AppliesDetraction,
        string? StatusCode
    ) : IRequest<PlantaResponse>;
}
