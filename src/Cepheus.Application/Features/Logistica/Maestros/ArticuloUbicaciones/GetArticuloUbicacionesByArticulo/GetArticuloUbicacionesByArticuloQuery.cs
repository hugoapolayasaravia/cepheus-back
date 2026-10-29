using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionesByArticulo
{
    public record GetArticuloUbicacionesByArticuloQuery(
        string ArticuloCode
    ) : IRequest<List<ArticuloUbicacionesResponse>>;
}