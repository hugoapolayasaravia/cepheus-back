using MediatR;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.ToggleMotivoDevolucionStatus
{
    public record ToggleMotivoDevolucionStatusCommand(string Code) : IRequest<bool>;
}
