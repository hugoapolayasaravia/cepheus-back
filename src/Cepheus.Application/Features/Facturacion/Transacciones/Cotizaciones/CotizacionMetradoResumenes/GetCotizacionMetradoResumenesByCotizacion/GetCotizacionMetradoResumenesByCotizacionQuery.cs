using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.GetCotizacionMetradoResumenesByCotizacion
{
    public record GetCotizacionMetradoResumenesByCotizacionQuery(
        string NegocioCode, string Year, string Month, string Code
    ) : IRequest<List<CotizacionMetradoResumenResponse>>;
}
