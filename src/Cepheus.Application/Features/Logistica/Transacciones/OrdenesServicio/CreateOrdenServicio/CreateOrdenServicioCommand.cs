using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.CreateOrdenServicio;

/// <summary>
/// Registro de una Orden de Servicio (nace Pendiente). El correlativo (6XXXXX), la fecha de proceso, el tipo
/// de cambio (solo dólares: TC venta del día de la fecha de emisión) y los importes (Total, IGV, Monto) los
/// calcula el servidor. Moneda y forma de pago se envían (el front las precarga desde el proveedor).
/// Requiere al menos una línea. La Nota de Crédito (comprobante 07) aún no está disponible.
/// </summary>
public sealed class CreateOrdenServicioCommand : IRequest<OrdenServicioResponse>
{
    public string PlantaCode { get; set; } = default!;
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
    public List<OrdenServicioDetalleRequest> Detalles { get; set; } = new();
}
