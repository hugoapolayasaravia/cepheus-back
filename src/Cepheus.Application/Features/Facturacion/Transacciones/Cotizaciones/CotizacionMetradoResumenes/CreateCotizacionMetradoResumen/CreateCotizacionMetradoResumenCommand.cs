using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.CreateCotizacionMetradoResumen
{
    public record CreateCotizacionMetradoResumenCommand(
        string NegocioCode, string Year, string Month, string Code,
        int LevelNumber,
        string LevelName,
        string AlturaLosaCode,
        string OverloadOrShortage,
        decimal LinealMeters,
        decimal TotalVaults,
        decimal TotalMeters,
        decimal TotalPrice,
        decimal PricePerM2,
        decimal Quantity,
        string BuildingLevel,
        bool HasTransport,
        decimal TotalVaultsAlt,
        decimal TotalMetersAlt,
        decimal TotalPriceAlt,
        decimal PricePerM2Alt,
        bool HasMinPrice,
        decimal MinTotal,
        decimal MinTotalAlt,
        decimal MinTotalB,
        decimal MinTotalAltB,
        bool HasTransportB,
        bool HasMinPriceB
    ) : IRequest<CotizacionMetradoResumenResponse>;
}
