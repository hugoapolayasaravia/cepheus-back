using Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.CreateEspecialidad
{
    public record CreateEspecialidadTrabajadorCommand(
        string Name
    ) : IRequest<EspecialidadTrabajadorResponse>;
}