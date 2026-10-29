using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.CreateNotaDebitoVenta
{
    /// <summary>
    /// Registra una nota de débito Pendiente. La planta, el cliente y la obra son obligatorios; el documento
    /// que se debita es opcional (factura 01 o boleta 03 del mismo cliente; sin documento: null o "00").
    /// La fecha no puede ser futura y el tipo de cambio sale de la tabla de tipos de cambio a esa fecha.
    /// </summary>
    public record CreateNotaDebitoVentaCommand(
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
        string? DocumentTypeCode,
        string? DocumentNumber,
        List<NotaDebitoVentaDetalleInput> Details
    ) : IRequest<NotaDebitoVentaResponse>;
}
