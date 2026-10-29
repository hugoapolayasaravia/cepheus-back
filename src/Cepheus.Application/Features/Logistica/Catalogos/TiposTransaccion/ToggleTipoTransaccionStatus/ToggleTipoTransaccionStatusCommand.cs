using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.ToggleTipoTransaccionStatus
{
    public record ToggleTipoTransaccionStatusCommand(string Code) : IRequest<bool>;
}
