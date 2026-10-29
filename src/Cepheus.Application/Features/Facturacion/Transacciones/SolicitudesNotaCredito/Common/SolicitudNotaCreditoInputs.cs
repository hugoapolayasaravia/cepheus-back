namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common
{
    public record SolicitudNotaCreditoDetalleInput(
        string TipoProductoCode,
        string ProductoCode,
        int? LineKey,
        decimal Quantity,
        decimal UnitPrice,
        string? OrderCode
    );

    public record SolicitudNotaCreditoGuiaInput(
        string GuideNumber,
        decimal Quantity
    );

    /// <summary>
    /// Datos comunes de entrada de la solicitud (Create y Update), para compartir validación.
    /// </summary>
    public interface ISolicitudNotaCreditoInput
    {
        string NegocioCode { get; }
        DateTime IssueDate { get; }
        string ClienteCode { get; }
        string DocumentTypeCode { get; }
        string DocumentNumber { get; }
        string MotivoCode { get; }
        string MonedaCode { get; }
        string Observation { get; }
        List<SolicitudNotaCreditoDetalleInput> Details { get; }
        List<SolicitudNotaCreditoGuiaInput>? Guides { get; }
    }
}
