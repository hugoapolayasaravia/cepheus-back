using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.ToggleMotivoDevolucionArticuloStatus
{
    public record ToggleMotivoDevolucionArticuloStatusCommand(string Code) : IRequest<bool>;
}
