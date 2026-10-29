using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.UpdateCotizacion
{
    /// <summary>
    /// Edita la cabecera. Solo permitido mientras la cotización esté Pendiente
    /// y sea modificable (IsEditable). No modifica Status, montos ni datos de
    /// aprobación/anulación (tienen sus propios comandos).
    /// </summary>
    public record UpdateCotizacionCommand(
        string NegocioCode,
        string Year,
        string Month,
        string Code,
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
        string? Observations,
        byte[] RowVersion
    ) : IRequest<CotizacionResponse>;
}
