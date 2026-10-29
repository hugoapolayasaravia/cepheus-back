using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GenerateNotaCreditoVentaFromSolicitud
{
    /// <summary>
    /// Genera la nota de crédito a partir de una Solicitud de Nota de Crédito con las 5 etapas aprobadas.
    /// Lo no indicado se completa con la solicitud y con los datos de la obra.
    /// </summary>
    public record GenerateNotaCreditoVentaFromSolicitudCommand(
        string SolicitudYear,
        string SolicitudMonth,
        string SolicitudCode,
        string PlantaCode,
        DateTime IssueDate,
        string ObraCode,
        string? VendedorCode,
        string? FormaPagoVentaCode,
        string? DeliveryAddress,
        string? DeliveryUbigeoCode,
        string? Description,
        string? Type,
        string? ExecutingPlantaCode,
        string? CashDocument
    ) : IRequest<NotaCreditoVentaResponse>;
}
