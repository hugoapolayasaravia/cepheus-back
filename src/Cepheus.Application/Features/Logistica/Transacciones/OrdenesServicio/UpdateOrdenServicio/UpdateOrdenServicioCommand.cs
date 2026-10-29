using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.UpdateOrdenServicio;

/// <summary>
/// Modificación de la cabecera. Las líneas se gestionan con los comandos de detalle.
///
/// Según el estado (TabSequence del PowerBuilder):
///   Pendiente          -> todos los campos son editables.
///   Aprobado/Procesado -> comprobante, proveedor, moneda y forma de pago quedan bloqueados.
///   Anulado/Cerrado, o con asiento contable -> no se modifica.
/// Los importes (Igv, NoGravable, Renta, Fonavi, Servicio, IgvExterior) solo se toman en la medida en que
/// el comprobante los habilita; los demás quedan en 0. Total y Monto los calcula el servidor.
/// </summary>
public sealed class UpdateOrdenServicioCommand : IRequest<OrdenServicioResponse>
{
    public string PlantaCode { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string ComprobantePagoCode { get; set; } = default!;
    public string? NumeroDocumento { get; set; }
    public string ProveedorCode { get; set; } = default!;
    public string MonedaCode { get; set; } = default!;
    public string FormaPagoCode { get; set; } = default!;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaRecepcion { get; set; }
    public string CompradorCode { get; set; } = default!;
    public string LugarEnvioCode { get; set; } = default!;
    public string TramiteCode { get; set; } = default!;
    public string? NotaCompraCode { get; set; }
    public string UnidadNegocioCode { get; set; } = default!;
    public string TrabajadorCode { get; set; } = default!;
    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }
    public decimal Igv { get; set; }
    public decimal NoGravable { get; set; }
    public decimal Renta { get; set; }
    public decimal Fonavi { get; set; }
    public decimal Servicio { get; set; }
    public decimal IgvExterior { get; set; }
}
