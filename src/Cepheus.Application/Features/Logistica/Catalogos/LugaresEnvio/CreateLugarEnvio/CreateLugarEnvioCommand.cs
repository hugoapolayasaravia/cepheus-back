using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.CreateLugarEnvio
{
    public record CreateLugarEnvioCommand(
        string Name,
        string? Address
    ) : IRequest<LugarEnvioResponse>;
}