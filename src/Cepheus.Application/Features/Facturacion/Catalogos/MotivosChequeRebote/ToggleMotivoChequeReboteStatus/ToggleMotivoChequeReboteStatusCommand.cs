using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.ToggleMotivoChequeReboteStatus
{
    public record ToggleMotivoChequeReboteStatusCommand(string Code) : IRequest<bool>;
}
