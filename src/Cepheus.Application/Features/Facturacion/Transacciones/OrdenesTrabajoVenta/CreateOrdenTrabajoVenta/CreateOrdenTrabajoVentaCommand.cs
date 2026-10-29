using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CreateOrdenTrabajoVenta;

/// <summary>
/// Registro de una OT de Ventas (nace Pendiente, Activa y modificable; el código es el correlativo global).
/// Tipos: Normal, Anticipo o Descuento (este último exige OriginCode = OT de Anticipo).
/// La cotización es opcional: si se indica, las líneas consumen su saldo y toman su precio.
/// </summary>
public sealed class CreateOrdenTrabajoVentaCommand : OrdenTrabajoVentaHeaderRequest, IRequest<OrdenTrabajoVentaResponse>
{
    public List<OrdenTrabajoVentaDetalleRequest> Detalles { get; set; } = new();
}
