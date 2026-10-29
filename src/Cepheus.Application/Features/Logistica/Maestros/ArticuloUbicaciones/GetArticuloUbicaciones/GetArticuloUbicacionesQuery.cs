using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicaciones
{
    public record GetArticuloUbicacionesQuery : IRequest<List<ArticuloUbicacionesResponse>>;
}