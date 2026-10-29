using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.GetTramiteByCode
{
    public record GetTramiteByCodeQuery(string Code) : IRequest<TramiteResponse>;
}