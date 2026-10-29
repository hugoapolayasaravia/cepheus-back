using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.ToggleSituacionEpsStatus
{
    public record ToggleSituacionEpsStatusCommand(string Code) : IRequest<bool>;
}