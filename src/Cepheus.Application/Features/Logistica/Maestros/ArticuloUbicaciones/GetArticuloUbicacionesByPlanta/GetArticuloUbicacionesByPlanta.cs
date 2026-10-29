using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionesByPlanta
{
    public record GetArticuloUbicacionesByPlantaQuery(
        string PlantaCode
    ) : IRequest<List<ArticuloUbicacionesResponse>>;
}