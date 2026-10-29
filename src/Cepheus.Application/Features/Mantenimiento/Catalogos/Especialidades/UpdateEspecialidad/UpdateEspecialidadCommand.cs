using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.UpdateEspecialidad
{
    public record UpdateEspecialidadCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<EspecialidadResponse>;
}
