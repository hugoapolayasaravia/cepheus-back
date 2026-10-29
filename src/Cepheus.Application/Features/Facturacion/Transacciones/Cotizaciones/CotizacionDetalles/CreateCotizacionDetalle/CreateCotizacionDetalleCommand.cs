using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.CreateCotizacionDetalle
{
    public record CreateCotizacionDetalleCommand(
        string NegocioCode,
        string Year,
        string Month,
        string Code,
        string ProductoTipoCode,
        string ProductoCode,
        string UnitCode,
        decimal Quantity,
        decimal UnitPrice,
        string Observations,
        int Order
    ) : IRequest<CotizacionDetalleResponse>;
}
