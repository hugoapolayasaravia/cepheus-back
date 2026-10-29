using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.RechazarEtapaSolicitudNotaCredito
{
    public record RechazarEtapaSolicitudNotaCreditoCommand(
        string Year,
        string Month,
        string Code,
        string Stage,
        string? Observation
    ) : IRequest<SolicitudNotaCreditoResponse>;
}
