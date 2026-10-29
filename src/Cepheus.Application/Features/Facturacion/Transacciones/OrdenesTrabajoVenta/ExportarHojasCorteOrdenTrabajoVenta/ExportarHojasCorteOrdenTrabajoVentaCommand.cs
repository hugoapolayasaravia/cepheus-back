using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.ExportarHojasCorteOrdenTrabajoVenta;

public sealed class ExportarHojasCorteRequest
{
    /// <summary>Hojas de Corte Pendientes del mismo cliente y obra que la OT.</summary>
    public List<HojaCorteReferenceRequest> Hojas { get; set; } = new();

    /// <summary>Precio unitario por cada producto resultante (las Hojas de Corte no traen precio).</summary>
    public List<HojaCortePrecioRequest> Precios { get; set; } = new();
}

/// <summary>
/// Exporta Hojas de Corte a la OT (PB ue_set_btn_grb_data_hc): agrega una línea por producto (negocio PT),
/// pasa cada Hoja de Corte a Procesado (13) y la vincula a la OT. Cantidad: vigueta = metros lineales totales
/// (largo x cantidad); bovedilla = cantidad.
/// </summary>
public sealed record ExportarHojasCorteOrdenTrabajoVentaCommand(string Code, ExportarHojasCorteRequest Data)
    : IRequest<OrdenTrabajoVentaResponse>;
