using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.ToggleOficinaStatus
{
    public record ToggleOficinaStatusCommand(string Code) : IRequest<bool>;
}