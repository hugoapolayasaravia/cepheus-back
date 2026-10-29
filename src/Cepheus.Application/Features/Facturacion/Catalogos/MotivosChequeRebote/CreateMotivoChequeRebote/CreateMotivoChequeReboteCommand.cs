using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.CreateMotivoChequeRebote
{
    public record CreateMotivoChequeReboteCommand(string Name) : IRequest<MotivoChequeReboteResponse>;
}
