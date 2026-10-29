using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.DeleteCotizacionDetalle
{
    public record DeleteCotizacionDetalleCommand(
        string NegocioCode, string Year, string Month, string Code, int Item
    ) : IRequest;
}
