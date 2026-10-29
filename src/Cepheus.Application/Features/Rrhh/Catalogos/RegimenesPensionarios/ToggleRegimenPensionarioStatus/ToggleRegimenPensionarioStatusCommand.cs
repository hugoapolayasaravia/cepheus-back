using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.ToggleRegimenPensionarioStatus
{
    public record ToggleRegimenPensionarioStatusCommand(string Code) : IRequest<bool>;
}