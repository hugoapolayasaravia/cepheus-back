using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.GetCotizacionMetradoDetallesByResumen
{
    public record GetCotizacionMetradoDetallesByResumenQuery(
        string NegocioCode, string Year, string Month, string Code, int LevelNumber
    ) : IRequest<List<CotizacionMetradoDetalleResponse>>;
}
