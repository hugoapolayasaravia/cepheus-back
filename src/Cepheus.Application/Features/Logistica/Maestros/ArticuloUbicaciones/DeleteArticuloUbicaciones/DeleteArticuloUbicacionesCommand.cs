using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.DeleteArticuloUbicaciones
{
    public record DeleteArticuloUbicacionesCommand(
        string Code
    ) : IRequest;
}