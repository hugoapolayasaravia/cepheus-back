using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.AnularNotaCreditoVenta
{
    public record AnularNotaCreditoVentaCommand(string PlantaCode, string Number, string Reason)
        : IRequest<NotaCreditoVentaResponse>;
}
