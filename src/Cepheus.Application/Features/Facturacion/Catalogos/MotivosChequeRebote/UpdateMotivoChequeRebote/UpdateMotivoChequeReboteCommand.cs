using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.UpdateMotivoChequeRebote
{
    public record UpdateMotivoChequeReboteCommand(string Code, string Name, byte[] RowVersion)
        : IRequest<MotivoChequeReboteResponse>;
}
