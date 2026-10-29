using Cepheus.Application.Features.Comunes.Negocios.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.GetNegocioByCode
{
    public record GetNegocioByCodeQuery(string Code) : IRequest<NegocioResponse>;
}
