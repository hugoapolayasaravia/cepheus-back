using Cepheus.Application.Features.Logistica.Maestros.Articulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.GetArticuloByCode
{
    public record GetArticuloByCodeQuery(string Code) : IRequest<ArticuloResponse>;
}
