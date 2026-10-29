using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.GetFormaPagoVentaByCode
{
    public record GetFormaPagoVentaByCodeQuery(string Code) : IRequest<FormaPagoVentaResponse>;
}
