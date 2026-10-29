using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.GetMotivoChequeReboteByCode
{
    public record GetMotivoChequeReboteByCodeQuery(string Code) : IRequest<MotivoChequeReboteResponse>;
}
