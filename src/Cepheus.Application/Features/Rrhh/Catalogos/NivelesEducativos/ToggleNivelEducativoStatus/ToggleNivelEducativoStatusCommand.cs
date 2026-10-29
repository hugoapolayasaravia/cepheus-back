using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.ToggleNivelEducativoStatus
{
    public record ToggleNivelEducativoStatusCommand(string Code) : IRequest<bool>;
}