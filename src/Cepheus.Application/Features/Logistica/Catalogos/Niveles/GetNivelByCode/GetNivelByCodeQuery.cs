using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.GetNivelByCode
{
    public record GetNivelByCodeQuery(string Code) : IRequest<NivelResponse>;
}
