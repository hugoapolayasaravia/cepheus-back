using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.CreateMaquina
{
    public record CreateMaquinaCommand(
        string Code,
        string Name
    ) : IRequest<MaquinaResponse>;
}
