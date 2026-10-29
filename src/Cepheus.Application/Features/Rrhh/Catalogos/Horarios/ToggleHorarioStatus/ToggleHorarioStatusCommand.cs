using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.ToggleHorarioStatus
{
    public record ToggleHorarioStatusCommand(string Code) : IRequest<bool>;
}