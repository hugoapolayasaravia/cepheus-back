using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.CreateFalla
{
    public record CreateFallaCommand(
        string Code,
        string Name
    ) : IRequest<FallaResponse>;
}
