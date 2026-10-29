using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionById
{
    public record GetArticuloUbicacionByIdQuery(
        string Code
    ) : IRequest<ArticuloUbicacionesResponse>;
}