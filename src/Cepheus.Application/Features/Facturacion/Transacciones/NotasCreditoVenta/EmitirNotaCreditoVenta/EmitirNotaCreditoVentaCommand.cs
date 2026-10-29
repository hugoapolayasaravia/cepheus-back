using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.EmitirNotaCreditoVenta
{
    public record EmitirNotaCreditoVentaCommand(string PlantaCode, string Number) : IRequest<NotaCreditoVentaResponse>;
}
