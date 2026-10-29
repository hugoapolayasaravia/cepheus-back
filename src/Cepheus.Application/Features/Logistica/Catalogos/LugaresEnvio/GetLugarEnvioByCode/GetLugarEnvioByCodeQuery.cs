using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.GetLugarEnvioByCode
{
    public record GetLugarEnvioByCodeQuery(string Code) : IRequest<LugarEnvioResponse>;
}