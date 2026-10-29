using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobacionEspecialOrdenTrabajoVenta;

public sealed class AprobacionEspecialOrdenTrabajoVentaRequest
{
    /// <summary>true = Créditos concede la excepción de línea de crédito; false = la rechaza.</summary>
    public bool Granted { get; set; }

    /// <summary>Sustento de la decisión. Obligatorio.</summary>
    public string Observations { get; set; } = default!;
}

/// <summary>
/// Aprobación especial de Créditos (PB: w_reg_lim_cre_ot): cuando la línea de crédito no cubre la OT, Créditos puede
/// conceder la excepción. Concedida, la OT deja de validar crédito y vuelve a ser modificable para completar las líneas.
/// </summary>
public sealed record AprobacionEspecialOrdenTrabajoVentaCommand(string Code, AprobacionEspecialOrdenTrabajoVentaRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
