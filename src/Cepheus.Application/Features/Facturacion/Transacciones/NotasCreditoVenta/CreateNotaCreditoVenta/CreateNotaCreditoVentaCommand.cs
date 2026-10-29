using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.CreateNotaCreditoVenta
{
    public record CreateNotaCreditoVentaCommand(
        string PlantaCode,
        DateTime IssueDate,
        string ClienteCode,
        string ObraCode,
        string? VendedorCode,
        string? DeliveryAddress,
        string? DeliveryUbigeoCode,
        string? FormaPagoVentaCode,
        string MonedaCode,
        string? OrderCode,
        string Description,
        string? ExecutingPlantaCode,
        string? Type,
        string DocumentTypeCode,
        string DocumentNumber,
        string MotivoDevolucionCode,
        string? CashDocument,
        List<NotaCreditoVentaDetalleInput> Details
    ) : IRequest<NotaCreditoVentaResponse>;
}
