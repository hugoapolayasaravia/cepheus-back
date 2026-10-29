using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.UpdateEspecialidad
{
    public record UpdateEspecialidadTrabajadorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<EspecialidadTrabajadorResponse>;
}