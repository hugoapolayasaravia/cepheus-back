using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.UpdateArticuloPlantaUbicaciones
{
    public record UpdateArticuloPlantaUbicacionesCommand(
        string ArticuloCode,
        string PlantaCode,
        string ArticuloUbicacionCode
    ) : IRequest<ArticuloPlantaUbicacionesResponse>;
}