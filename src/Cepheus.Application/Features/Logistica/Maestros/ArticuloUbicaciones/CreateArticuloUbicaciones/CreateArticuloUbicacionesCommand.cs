using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.CreateArticuloUbicaciones
{
    public record CreateArticuloUbicacionesCommand(
    string Name,
    string PlantaCode,
    string? Zone,
    string? Rack,
    string? Shelf,
    string? Locker
) : IRequest<ArticuloUbicacionesResponse>;
}
