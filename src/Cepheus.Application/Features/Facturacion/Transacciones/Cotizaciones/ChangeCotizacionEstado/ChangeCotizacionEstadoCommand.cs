using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.ChangeCotizacionEstado
{
    /// <summary>
    /// Cambia el estado a Pendiente, Aceptado, Proyectado o NoAceptado.
    /// Aprobado y Anulado tienen comandos propios (ApproveCotizacionCommand,
    /// AnularCotizacionCommand) porque completan campos adicionales
    /// (ApprovedBy/At, CancelReason/CanceledBy/At) que este comando genérico
    /// no debe tocar.
    /// </summary>
    public record ChangeCotizacionEstadoCommand(
        string NegocioCode, string Year, string Month, string Code, string NuevoEstado
    ) : IRequest<CotizacionResponse>;
}
