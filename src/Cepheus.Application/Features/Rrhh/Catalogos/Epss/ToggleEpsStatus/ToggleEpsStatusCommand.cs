using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.ToggleEpsStatus
{
    public record ToggleEpsStatusCommand(string Code) : IRequest<bool>;
}