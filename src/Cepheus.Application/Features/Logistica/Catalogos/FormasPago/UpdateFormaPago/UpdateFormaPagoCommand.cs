using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.UpdateFormaPago
{
    public record UpdateFormaPagoCommand(
        string Code,
        string Name,
        int Days,
        bool IsCredit,
        byte[] RowVersion
    ) : IRequest<FormaPagoResponse>;
}