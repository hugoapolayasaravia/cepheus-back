using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.CreateNotaIngreso;

/// <summary>
/// Registro de una Nota de Ingreso.
///
/// Flujo normal (comprobante distinto de Nota de Crédito): se recibe contra una Orden de Compra
/// (OrdenCompraCode). Proveedor, moneda y forma de pago vienen de la OC; el tipo de cambio se resuelve
/// por la fecha de emisión. Actualiza lo entregado en la OC (por línea y por pedido de origen) y el stock.
///
/// Nota de Crédito (comprobante 07): se registra contra la Nota de Ingreso del documento referenciado
/// (ComprobantePagoReferenciaId + ProveedorCode + NumeroReferencia) y requiere MotivoDevolucionId
/// (01 ajuste de precio/descuento, no mueve stock; 02 devolución, descuenta stock).
///
/// Pendiente (se rechaza explícitamente): Condicion = AnexarGuias y Origen = Importacion / Transferencia.
/// </summary>
public sealed class CreateNotaIngresoCommand : IRequest<NotaIngresoResponse>
{
    public string PlantaCode { get; set; } = default!;
    public CondicionNotaIngreso Condicion { get; set; } = CondicionNotaIngreso.OrdenCompra;
    public OrigenNotaIngreso Origen { get; set; } = OrigenNotaIngreso.Compra;

    public string? ComprobantePagoCode { get; set; }
    public string? OrdenCompraCode { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? NumeroGuia { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaRecepcion { get; set; }

    // Solo Nota de Crédito
    public string? ProveedorCode { get; set; }
    public string? ComprobantePagoReferenciaCode { get; set; }
    public string? NumeroReferencia { get; set; }
    public string? MotivoDevolucionCode { get; set; }

    public List<NotaIngresoDetalleRequest> Detalles { get; set; } = new();
}
