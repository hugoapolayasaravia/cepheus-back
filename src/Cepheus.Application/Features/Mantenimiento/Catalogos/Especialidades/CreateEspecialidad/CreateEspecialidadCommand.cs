using Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.CreateEspecialidad
{
    public record CreateEspecialidadCommand(
        string Code,
        string Name
    ) : IRequest<EspecialidadResponse>;
}
