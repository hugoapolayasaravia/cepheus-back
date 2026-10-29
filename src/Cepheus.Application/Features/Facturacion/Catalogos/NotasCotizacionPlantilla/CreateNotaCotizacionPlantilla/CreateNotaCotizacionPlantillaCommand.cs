using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.CreateNotaCotizacionPlantilla
{
    public record CreateNotaCotizacionPlantillaCommand(
        string NegocioCode,
        string Description,
        OpcionNotaCotizacion Option
    ) : IRequest<NotaCotizacionPlantillaResponse>;
}
