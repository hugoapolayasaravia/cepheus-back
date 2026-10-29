using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.GetArticuloPlantaUbicacionesByKey
{
    public record GetArticuloPlantaUbicacionesByKeyQuery(
        string ArticuloCode,
        string PlantaCode
    ) : IRequest<ArticuloPlantaUbicacionesResponse>;

    
}