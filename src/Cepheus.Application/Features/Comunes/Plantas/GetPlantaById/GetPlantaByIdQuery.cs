using Cepheus.Application.Features.Comunes.Plantas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.GetPlantaById
{
    public record GetPlantaByIdQuery(string Code) : IRequest<PlantaResponse>;
}
