using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.UpdateArticuloUbicaciones
{
    public record UpdateArticuloUbicacionesCommand(
        string Code,
        string Name,
        string PlantaCode,
        string? Zone,
        string? Rack,
        string? Shelf,
        string? Locker,
        bool IsActive
    ) : IRequest<ArticuloUbicacionesResponse>;
}