using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.GetCotizacionDetallesByCotizacion
{
    public record GetCotizacionDetallesByCotizacionQuery(
        string NegocioCode, string Year, string Month, string Code
    ) : IRequest<List<CotizacionDetalleResponse>>;
}
