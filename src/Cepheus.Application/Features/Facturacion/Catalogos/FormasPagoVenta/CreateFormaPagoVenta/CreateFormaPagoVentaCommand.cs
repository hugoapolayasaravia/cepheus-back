using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.CreateFormaPagoVenta
{
    public record CreateFormaPagoVentaCommand(
        string Name,
        int Days,
        bool IsCredit
    ) : IRequest<FormaPagoVentaResponse>;
}
