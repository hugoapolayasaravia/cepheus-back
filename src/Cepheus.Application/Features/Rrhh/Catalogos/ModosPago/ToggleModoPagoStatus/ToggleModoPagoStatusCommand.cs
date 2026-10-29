using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.ToggleModoPagoStatus
{
    public record ToggleModoPagoStatusCommand(string Code) : IRequest<bool>;
}