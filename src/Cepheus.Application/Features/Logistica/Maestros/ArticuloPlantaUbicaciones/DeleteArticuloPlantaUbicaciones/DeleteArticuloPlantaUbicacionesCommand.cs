using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.DeleteArticuloPlantaUbicaciones
{
    public record DeleteArticuloPlantaUbicacionesCommand(
        string ArticuloCode,
        string PlantaCode
    ) : IRequest;
}