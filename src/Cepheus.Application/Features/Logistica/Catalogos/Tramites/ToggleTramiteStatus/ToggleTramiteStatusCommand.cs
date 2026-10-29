using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.ToggleTramiteStatus
{
    public record ToggleTramiteStatusCommand(string Code) : IRequest<bool>;
}