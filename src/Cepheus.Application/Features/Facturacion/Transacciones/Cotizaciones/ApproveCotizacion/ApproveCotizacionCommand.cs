using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.ApproveCotizacion
{
    public record ApproveCotizacionCommand(
        string NegocioCode, string Year, string Month, string Code
    ) : IRequest<CotizacionResponse>;
}
