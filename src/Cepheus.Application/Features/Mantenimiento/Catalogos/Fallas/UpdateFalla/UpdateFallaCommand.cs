using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.UpdateFalla
{
    public record UpdateFallaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<FallaResponse>;
}
