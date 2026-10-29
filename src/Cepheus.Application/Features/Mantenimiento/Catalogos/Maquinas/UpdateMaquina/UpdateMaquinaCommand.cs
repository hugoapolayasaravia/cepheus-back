using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.UpdateMaquina
{
    public record UpdateMaquinaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<MaquinaResponse>;
}
