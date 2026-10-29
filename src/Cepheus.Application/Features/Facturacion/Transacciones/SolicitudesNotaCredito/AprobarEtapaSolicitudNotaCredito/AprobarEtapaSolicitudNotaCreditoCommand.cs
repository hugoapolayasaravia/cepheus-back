using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AprobarEtapaSolicitudNotaCredito
{
    public record AprobarEtapaSolicitudNotaCreditoCommand(
        string Year,
        string Month,
        string Code,
        string Stage,
        string? Observation
    ) : IRequest<SolicitudNotaCreditoResponse>;
}
