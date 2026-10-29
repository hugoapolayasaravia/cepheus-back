using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.InactivarOrdenTrabajoVenta;

public sealed class InactivarOrdenTrabajoVentaRequest
{
    /// <summary>Motivo (PB: Observacion_Ina, mínimo 2 caracteres). Obligatorio.</summary>
    public string Reason { get; set; } = default!;
}

/// <summary>Inactiva una OT Activa (PB: Codigo_est 05 -> 06 con motivo).</summary>
public sealed record InactivarOrdenTrabajoVentaCommand(string Code, InactivarOrdenTrabajoVentaRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
