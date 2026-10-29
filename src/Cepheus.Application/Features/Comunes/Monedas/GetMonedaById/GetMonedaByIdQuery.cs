using Cepheus.Application.Features.Comunes.Monedas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Monedas.GetMonedaById
{
    public record GetMonedaByIdQuery(string Code) : IRequest<MonedaResponse>;
}
