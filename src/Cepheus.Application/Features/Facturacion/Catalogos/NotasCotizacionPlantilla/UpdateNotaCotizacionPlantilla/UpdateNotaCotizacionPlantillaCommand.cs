using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.UpdateNotaCotizacionPlantilla
{
    public record UpdateNotaCotizacionPlantillaCommand(
        string NegocioCode,
        string Code,
        string Description,
        OpcionNotaCotizacion Option,
        byte[] RowVersion
    ) : IRequest<NotaCotizacionPlantillaResponse>;
}
