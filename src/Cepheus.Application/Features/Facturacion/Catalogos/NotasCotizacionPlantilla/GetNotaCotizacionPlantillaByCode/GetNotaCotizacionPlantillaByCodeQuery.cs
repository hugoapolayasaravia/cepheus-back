using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.GetNotaCotizacionPlantillaByCode
{
    public record GetNotaCotizacionPlantillaByCodeQuery(string NegocioCode, string Code) : IRequest<NotaCotizacionPlantillaResponse>;
}
