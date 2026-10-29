using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.UpdateCotizacionDetalle
{
    public record UpdateCotizacionDetalleCommand(
        string NegocioCode,
        string Year,
        string Month,
        string Code,
        int Item,
        string UnitCode,
        decimal Quantity,
        decimal UnitPrice,
        string Observations,
        int Order,
        byte[] RowVersion
    ) : IRequest<CotizacionDetalleResponse>;
}
