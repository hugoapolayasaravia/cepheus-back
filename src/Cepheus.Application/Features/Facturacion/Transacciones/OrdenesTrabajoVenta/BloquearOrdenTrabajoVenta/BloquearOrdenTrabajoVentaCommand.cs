using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.BloquearOrdenTrabajoVenta;

/// <summary>
/// Bloquea la OT para aprobación (PB: Ind_Modificable = 'N'). Si la condición de pago de la obra es crédito, se aprueba
/// sola (aprobación automática y pase a producción); si es contado queda pendiente de la aprobación de Créditos.
/// </summary>
public sealed record BloquearOrdenTrabajoVentaCommand(string Code) : IRequest<OrdenTrabajoVentaResponse>;
