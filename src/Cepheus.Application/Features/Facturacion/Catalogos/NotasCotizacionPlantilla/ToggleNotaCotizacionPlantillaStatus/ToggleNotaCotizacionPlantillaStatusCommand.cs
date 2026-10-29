using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.ToggleNotaCotizacionPlantillaStatus
{
    public record ToggleNotaCotizacionPlantillaStatusCommand(string NegocioCode, string Code) : IRequest<bool>;
}
