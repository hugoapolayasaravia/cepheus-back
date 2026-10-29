using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GetNotaCreditoVentaByKey
{
    public record GetNotaCreditoVentaByKeyQuery(string PlantaCode, string Number) : IRequest<NotaCreditoVentaResponse>;
}
