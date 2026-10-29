using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.CreateCotizacionMetradoDetalle
{
    public record CreateCotizacionMetradoDetalleCommand(
        string NegocioCode, string Year, string Month, string Code, int LevelNumber,
        string Order,
        string ProductoTipoCode,
        string ProductoCode,
        string PanelCode,
        int Times,
        decimal InnerLength,
        decimal OuterLength,
        decimal Support,
        decimal Quantity,
        decimal TotalMaterial,
        decimal VaultCount,
        decimal WastePercentage,
        decimal Row,
        decimal QuantityB,
        decimal Support2,
        decimal Width,
        decimal Area,
        decimal MaterialPrice,
        decimal MaterialIgv,
        decimal TransportPrice,
        decimal VaultPrice,
        decimal VaultTotalPrice,
        decimal MaterialPriceAlt,
        decimal TransportPriceAlt,
        decimal VaultPriceAlt,
        decimal VaultTotalPriceAlt,
        string SortOrder,
        decimal DeliveredTotalMaterial,
        decimal DeliveredQuantityB,
        decimal PolystyrenePrice,
        decimal PolystyreneTotalPrice,
        decimal PolystyrenePriceAlt,
        decimal PolystyreneTotalPriceAlt,
        decimal Widening,
        decimal SupportP,
        decimal WastePercentageP,
        decimal QuantityP,
        string Anchorage,
        decimal Spacing
    ) : IRequest<CotizacionMetradoDetalleResponse>;
}
