using Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EspecialidadTrabajadores.CreateEspecialidad
{
    public record CreateEspecialidadTrabajadorCommand(
        string Name
    ) : IRequest<EspecialidadTrabajadorResponse>;
}