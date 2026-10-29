using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.GetPlantillaAsientoByCode
{
    public record GetPlantillaAsientoByCodeQuery(string Code) : IRequest<PlantillaAsientoResponse>;
}
