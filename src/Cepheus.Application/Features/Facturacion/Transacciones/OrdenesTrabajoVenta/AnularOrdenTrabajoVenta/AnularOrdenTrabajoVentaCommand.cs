using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularOrdenTrabajoVenta;

/// <summary>Cuerpo del DELETE: motivo de la anulación y destino de las Hojas de Corte vinculadas.</summary>
public sealed class AnularOrdenTrabajoVentaRequest
{
    /// <summary>Motivo (mínimo 2 caracteres). Obligatorio.</summary>
    public string Reason { get; set; } = default!;

    /// <summary>
    /// false (por defecto): las Hojas de Corte vinculadas se liberan y vuelven a Pendiente (se pueden exportar a otra OT).
    /// true: criterio del PB al anular, las Hojas de Corte pasan a Anulado (04).
    /// </summary>
    public bool AnularHojasCorte { get; set; }
}

public sealed record AnularOrdenTrabajoVentaCommand(string Code, AnularOrdenTrabajoVentaRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
