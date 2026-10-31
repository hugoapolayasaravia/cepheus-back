using Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.UpdateEspecialidad
{
    public record UpdateEspecialidadTrabajadorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<EspecialidadTrabajadorResponse>;
}