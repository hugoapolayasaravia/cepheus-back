using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.CreateFormaPago
{
    public record CreateFormaPagoCommand(
        string Name,
        int Days,
        bool IsCredit
    ) : IRequest<FormaPagoResponse>;
}