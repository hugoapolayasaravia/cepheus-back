using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.CreateCotizacion
{
    /// <summary>
    /// No recibe Year/Month/Code (se derivan de Date y se genera el
    /// correlativo en el backend), ni Status/GrossAmount/IgvAmount/NetAmount
    /// (una cotización nace Pendiente, en 0, sin detalle todavía — se agregan
    /// líneas con CreateCotizacionDetalleCommand).
    /// </summary>
    public record CreateCotizacionCommand(
        string NegocioCode,
        string VendedorCode,
        DateTime Date,
        string CurrencyCode,
        string FormaPagoVentaCode,
        string? TecnicoCode,
        bool AppliesIgv,
        decimal Discount,
        decimal GlobalVolume,
        string Type,
        string? MetradoCalculationSystem,
        int WorkDurationMonths,
        string? ClienteCode,
        string? Ruc,
        string ClientName,
        string? ClientAddress,
        string ClientAddressUbigeoCode,
        string? ObraCode,
        string WorkName,
        string ProjectStatus,
        string WorkAddressUbigeoCode,
        string WorkAddress,
        string ContactName,
        string ContactPhone,
        string ContactEmail,
        string? Reference,
        DateTime? StartDate,
        DateTime? EndDate,
        DateTime? DispatchDate,
        string FleteCode,
        decimal IgvRate,
        string? Observations
    ) : IRequest<CotizacionResponse>;
}
