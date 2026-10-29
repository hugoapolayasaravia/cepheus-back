using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CopiarOrdenTrabajoVenta;

/// <summary>
/// Copia cualquier OT (Normal, Anticipo o Descuento) en una OT nueva: Pendiente, Activa, modificable y sin
/// aprobación. Copia cabecera y líneas; NO copia cotización, Hojas de Corte, cantidades aprobadas/programadas/entregadas
/// ni observaciones de Créditos/Hoja de Corte. La OT de origen queda en OriginCode (en Descuento se conserva el anticipo).
/// </summary>
public sealed record CopiarOrdenTrabajoVentaCommand(string Code) : IRequest<OrdenTrabajoVentaResponse>;
