using Cepheus.Application.Features.Rrhh.Catalogos.Horarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.CreateHorario
{
    public record CreateHorarioCommand(
        string Name
    ) : IRequest<HorarioResponse>;
}