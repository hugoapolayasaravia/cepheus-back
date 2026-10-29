using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.UpdateFormaPagoVenta
{
    public record UpdateFormaPagoVentaCommand(
        string Code,
        string Name,
        int Days,
        bool IsCredit,
        byte[] RowVersion
    ) : IRequest<FormaPagoVentaResponse>;
}
