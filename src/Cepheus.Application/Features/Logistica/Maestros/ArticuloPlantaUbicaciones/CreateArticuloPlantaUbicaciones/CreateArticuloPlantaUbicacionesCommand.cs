using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.CreateArticuloPlantaUbicaciones
{
    public record CreateArticuloPlantaUbicacionesCommand(
        string ArticuloCode,
        string PlantaCode,
        string ArticuloUbicacionCode
    ) : IRequest<ArticuloPlantaUbicacionesResponse>;
}