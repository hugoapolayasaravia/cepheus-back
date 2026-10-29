using Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.UpdateLugarEnvio
{
    public record UpdateLugarEnvioCommand(
        string Code,
        string Name,
        string? Address,
        byte[] RowVersion
    ) : IRequest<LugarEnvioResponse>;
}