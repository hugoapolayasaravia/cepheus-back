using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularAprobacionOrdenTrabajoVenta;

public sealed class AnularAprobacionOrdenTrabajoVentaRequest
{
    /// <summary>Motivo (mínimo 2 caracteres). Obligatorio.</summary>
    public string Reason { get; set; } = default!;

    /// <summary>Observaciones de la anulación (opcional).</summary>
    public string? Observations { get; set; }
}

/// <summary>
/// Anula la aprobación (PB: w_ges_otrabajo_aprobacion): la OT vuelve a Pendiente y modificable, se devuelven los números de
/// documento a la planta (si eran los últimos) y se registra en la bitácora de aprobaciones anuladas.
/// </summary>
public sealed record AnularAprobacionOrdenTrabajoVentaCommand(string Code, AnularAprobacionOrdenTrabajoVentaRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
