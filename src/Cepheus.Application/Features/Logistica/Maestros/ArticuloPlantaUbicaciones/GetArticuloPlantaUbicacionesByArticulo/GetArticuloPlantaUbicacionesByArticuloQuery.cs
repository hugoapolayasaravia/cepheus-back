using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.GetArticuloPlantaUbicacionesByArticulo
{
    public record GetArticuloPlantaUbicacionesByArticuloQuery(
        string ArticuloCode
    ) : IRequest<List<ArticuloPlantaUbicacionesResponse>>;
}
